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
        Task<Tuple<List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>>> GetItensDeCasaAsync();
        Task<bool> UpdateItemDeCasa(ItemCasaModel item);
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

        public async Task<Tuple<List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>>> GetItensDeCasaAsync()
        {
            var result = await _supabase.From<ItemDeCasa>().Filter("ativo", Operator.Equals, "true").Get();
            var response = result.Models.OrderBy(x => x.Id);

            var listaModelada = GetListModelada(response.ToList());
            var camaLista = listaModelada.Item1.OrderBy(x => x.Id).ToList();
            var mesaLista = listaModelada.Item2.OrderBy(x => x.Id).ToList();
            var banhoLista = listaModelada.Item3.OrderBy(x => x.Id).ToList();
            var eletroLista = listaModelada.Item4.OrderBy(x => x.Id).ToList();
            var cozinhaLista = listaModelada.Item5.OrderBy(x => x.Id).ToList();

            return new Tuple<List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>>(camaLista, mesaLista, banhoLista, eletroLista, cozinhaLista);
        }

        private Tuple<List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>> GetListModelada(List<ItemDeCasa> itemDeCasas)
        {
            // Crie as cinco listas
            var camaList = new List<ItemCasaModel>();
            var mesaList = new List<ItemCasaModel>();
            var banhoList = new List<ItemCasaModel>();
            var eletroList = new List<ItemCasaModel>();
            var cozinhaList = new List<ItemCasaModel>();

            int camaInt = 0;
            int mesaInt = 0;
            int banhoInt = 0;
            int eletroInt = 0;
            int cozinhaInt = 0;
            foreach (var item in itemDeCasas)
            {
                if (item.Ambiente == 1)
                {
                    camaList.Add(new ItemCasaModel { PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas, Ambiente = item.Ambiente, Ativo = true, Id = camaInt, NomeItem = item.NomeItem });
                    camaInt++;
                }
                else if (item.Ambiente == 2)
                {
                    mesaList.Add(new ItemCasaModel { PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas, Ambiente = item.Ambiente, Ativo = true, Id = mesaInt, NomeItem = item.NomeItem });
                    mesaInt++;
                }
                else if (item.Ambiente == 3)
                {
                    banhoList.Add(new ItemCasaModel { PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas, Ambiente = item.Ambiente, Ativo = true, Id = banhoInt, NomeItem = item.NomeItem });
                    banhoInt++;
                }
                else if (item.Ambiente == 4)
                {
                    eletroList.Add(new ItemCasaModel { PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas, Ambiente = item.Ambiente, Ativo = true, Id = eletroInt, NomeItem = item.NomeItem });
                    eletroInt++;
                }
                else if (item.Ambiente == 5)
                {
                    cozinhaList.Add(new ItemCasaModel { PrimaryId = item.Id, Quantidade = item.Quantidade, NomePessoas = item.NomePessoas, Ambiente = item.Ambiente, Ativo = true, Id = cozinhaInt, NomeItem = item.NomeItem });
                    cozinhaInt++;
                }
            }
            // Crie e retorne a tupla com as listas preenchidas
            var tuple = new Tuple<List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>, List<ItemCasaModel>>(
                camaList, mesaList, banhoList, eletroList, cozinhaList);

            return tuple;
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
                      .Set(x => x.Quantidade, quantidade)
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
