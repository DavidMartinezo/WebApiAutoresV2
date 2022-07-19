namespace WebApiAutoresV2.Servicios
{
    public class escribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "archivo1.txt";
        private  Timer timer;
        public escribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(Dowork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            escribir("iniciando proceso");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            escribir("Deteniendo el proceso");
            return Task.CompletedTask;
        }
        public void escribir(string msg)
        {
            var ruta =$@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter sw = new StreamWriter(ruta,append: true))
            {
                sw.WriteLine(msg);
            } ;
        }

        private void Dowork(object state)
        {
            escribir("Proceso en Ejecucion " + DateTime.Now.ToString("dd/mm/yyyyy hh:mm:ss"));
        }
    }
}
