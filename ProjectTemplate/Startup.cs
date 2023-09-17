using DomainModel.Abstract;
using DomainModel.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTemplate
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            IMvcBuilder builder = services.AddRazorPages();
            services.AddScoped<IArticleRepository, ArticleRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

           /* app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });*/

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "test_page", pattern: "page_test", new { controller = "Home", action = "PageTest", page_name = "test_page" });

                //Main pages route. (To be transalted)
                endpoints.MapControllerRoute(name: "article_page", pattern: "article/edit/{id}", new { controller = "Article", action = "EditArticle", page_name = "article_page" });
                endpoints.MapControllerRoute(name: "all_articles", pattern: "article/articles", new { controller = "Article", action = "Articles", page_name = "all_articles" });

                //Ajax routes
                endpoints.MapControllerRoute(name: "article_page_creation", pattern: "article/create_new_article", new { controller = "Article", action = "CreateNewArticle", page_name = "article_page_creation" });

                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
