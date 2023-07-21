using Tic_Tac_Toe.Game;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddScoped<GameLogic>(); // Registrar o serviço GameLogic

            services.AddScoped<QLearning2>();
            services.AddScoped<QLearning>(provider =>
            {
                int numStates = 19683; // Total de estados possíveis no jogo da velha
                int numActions = provider.GetRequiredService<GameLogic>().CountEmptyCells();
                double learningRate = 0.1; // Taxa de aprendizado de 10%
                double discountFactor = 0.9; // Fator de desconto de 90%

                return new QLearning(numStates, numActions, learningRate, discountFactor);
            });

            services.AddControllers();
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            }
            else
            {
                app.UseCors();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
