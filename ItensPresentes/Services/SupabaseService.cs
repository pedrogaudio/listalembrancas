using ItensPresentes.Data;
using Supabase;
using static Postgrest.Constants;
using System.Linq;

namespace ItensPresentes.Services
{
    public interface ISupabaseService
    {
        Task InitializeAsync();
        Task<List<ItemDeCasa>> GetItensDeCasaAsync();
        Task<bool> UpdateItemDeCasa(ItemDeCasa item);
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

        public async Task<List<ItemDeCasa>> GetItensDeCasaAsync()
        {
            var result = await _supabase.From<ItemDeCasa>().Filter("ativo", Operator.Equals, "true").Get();
            var response = result.Models.OrderBy(x=> x.Id);

            return response.ToList();
        }

        public async Task<bool> UpdateItemDeCasa(ItemDeCasa item)
        {
            bool sucesso = false;
            try
            {
                long quantidade = 0;
                var result = await _supabase.From<ItemDeCasa>().Filter("id", Operator.Equals, item.Id.ToString()).Get();

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
                    quantidade = atual.Quantidade - 1;

                //atualizo
                var update = await _supabase.From<ItemDeCasa>()
                      .Where(x => x.Id == item.Id)
                      .Set(x => x.NomePessoas, pessoas)
                      .Set(x => x.Quantidade, quantidade == 0 ? item.Quantidade : quantidade)
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
