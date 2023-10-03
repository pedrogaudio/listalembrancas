using ItensPresentes.Data;
using Supabase;
using static Postgrest.Constants;
using System.Linq;
using ItensPresentes.Model;
using System;

namespace ItensPresentes.Services
{
    public interface ISupabaseService
    {
        Task InitializeAsync();
        Task<List<ItemCasaModel>> GetItensDeCasaAsync();
        Task<List<ItemCasaModel>> GetItensDeCasaAsync(int id);
        Task<bool> UpdateItemDeCasa(ItemCasaModel item);
        Task<List<ItemCasaModel>> GetPaginatedResult(int currentPage, int pageSize = 10);
        Task<int> GetCount();
    }

    public class SupabaseService : ISupabaseService
    {
        private readonly Client _supabase;

        public SupabaseService(string url, string key)
        {
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _supabase = new Client(url, key, options);
        }

        public async Task InitializeAsync()
        {
            await _supabase.InitializeAsync();
        }

        public async Task<List<ItemCasaModel>> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            var data = await GetItensDeCasaAsync();
            return data.OrderBy(d => d.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetCount()
        {
            var data = await GetItensDeCasaAsync();
            return data.Count;
        }

        public async Task<List<ItemCasaModel>> GetItensDeCasaAsync()
        {
            var result = await _supabase.From<ItemDeCasa>().Filter("ativo", Operator.Equals, "true").Get();
            var response = result.Models.OrderBy(x => x.NomeItem);

            return GetListModelada(response.ToList());
        }

        public async Task<List<ItemCasaModel>> GetItensDeCasaAsync(int id)
        {
            var result = await _supabase.From<ItemDeCasa>().Filter("id", Operator.Equals, id.ToString()).Get();
            var response = result.Models.OrderBy(x => x.NomeItem);

            return GetListModelada(response.ToList());
        }

        private List<ItemCasaModel> GetListModelada(List<ItemDeCasa> itemDeCasas)
        {
            var result = new List<ItemCasaModel>();

            int nro = 0;

            foreach (var item in itemDeCasas)
            {
                result.Add(new ItemCasaModel { Id = nro, Ativo = item.Ativo, Link = item.Link, NomeItem = item.NomeItem, PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas });
                nro++;
            }
            // Crie e retorne a tupla com as listas preenchidas
            return result;
        }

        public async Task<bool> UpdateItemDeCasa(ItemCasaModel item)
        {
            bool sucesso = false;
            try
            {
                long quantidade = 0;
                var result = await _supabase.From<ItemDeCasa>().Filter("id", Operator.Equals, item.PrimaryId.ToString()).Get();

                ItemDeCasa? atual = result?.Models?.FirstOrDefault();

                List<string> pessoas = new List<string>();

                if (atual?.NomePessoas?.Count > 0)
                {
                    pessoas.AddRange(from p in atual?.NomePessoas
                                     select p.ToString());
                }

                pessoas.AddRange(from p in item.NomePessoas
                                 select p.ToString());

                if (atual != null)
                    if (item.Quantidade > 0)
                        quantidade = atual.Quantidade - 1;

                //atualizo
                var update = await _supabase.From<ItemDeCasa>()
                      .Where(x => x.Id == item.PrimaryId)
                      .Set(x => x.NomePessoas, pessoas)
                      .Set(x => x.Quantidade, quantidade)
                      .Set(x => x.Ativo, quantidade == 0 ? false : true)
                      .Update();

                if (update.ResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    sucesso = true;
            }
            catch (Exception ex)
            {
                var teste = ex.Message;
            }
            return sucesso;
        }
    }
}
