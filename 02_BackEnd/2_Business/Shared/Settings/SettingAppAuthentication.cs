namespace Shared.Settings
{
    public class SettingAppAuthentication
    {
        public SettingAppAuthentication()
        {

        }

        public string Key { get; set; }
        public List<string> Policy { get; set; }
        public List<KeyValuePair<string, string[]>> PolicyList => SelectPolicys();

        private List<KeyValuePair<string, string[]>> SelectPolicys()
        {
            var listaRetorno = new List<KeyValuePair<string, string[]>>();

            if (Policy != null && Policy.Count > 0)
            {
                string[] listaPolicyParametroSplit;
                string chave;
                string[] valor;

                foreach (var item in Policy)
                {
                    listaPolicyParametroSplit = item.Split('|');
                    chave = listaPolicyParametroSplit[0];
                    valor = listaPolicyParametroSplit[1].Split(' ');

                    for (int i = 0; i < valor.Length; i++)
                    {
                        valor[i] = valor[i].Trim();
                    }

                    listaRetorno.Add(new KeyValuePair<string, string[]>(chave.Trim(), valor));
                }
            }

            return listaRetorno;
        }
    }
}
