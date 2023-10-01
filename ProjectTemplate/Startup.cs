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
            //services.AddControllers()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
            //    });
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
             }); */

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {

                //Main pages route. (To be transalted)
                endpoints.MapControllerRoute(name: "article_page", pattern: "article/edit/{id}", new { controller = "Article", action = "EditArticle", page_name = "article_page" });
                endpoints.MapControllerRoute(name: "all_articles", pattern: "article/articles", new { controller = "Article", action = "Articles", page_name = "all_articles" });
                              
                endpoints.MapControllerRoute(name: "test_page", pattern: "page_test", new { controller = "Home", action = "PageTest", page_name = "test_page" });

                //Ajax routes
                endpoints.MapControllerRoute(name: "article_page_creation", pattern: "article/create_new_article", new { controller = "Article", action = "CreateNewArticle", page_name = "article_page_creation" });
                endpoints.MapControllerRoute(name: "article_lookup_comments", pattern: "article/GetArticleComments", new { controller = "Article", action = "GetArticleComments", page_name = "article_lookup_comments" });
                endpoints.MapControllerRoute(name: "article_edit_comments", pattern: "article/edit_article_comments", new { controller = "Article", action = "EditArticleComments", page_name = "article_edit_comments" });
                endpoints.MapControllerRoute(name: "article_reply_comments", pattern: "article/reply_article_comments", new { controller = "Article", action = "ReplyArticleComments", page_name = "article_reply_comments" });
                endpoints.MapControllerRoute(name: "article_autoSave", pattern: "article/AutoSaveArticle", new { controller = "Article", action = "AutoSaveArticle", page_name = "article_autoSave" });
                endpoints.MapControllerRoute(name: "article_delete_comments", pattern: "article/delete_article_comments", new { controller = "Article", action = "DeleteArticleComments", page_name = "article_delete_comments" });
                
                endpoints.MapControllerRoute(name: "article_temp", pattern: "article/getArticleTemp", new { controller = "Article", action = "GetArticleTemp", page_name = "article_temp" });
                endpoints.MapControllerRoute(name: "delete_article_temp", pattern: "article/deleteArticleTemp", new { controller = "Article", action = "DeleteArticleTemp", page_name = "delete_article_temp" });

                endpoints.MapControllerRoute(name: "article_form_definition", pattern: "article/get_article_form_definition", new { controller = "Article", action = "GetArticleFormDefinition", page_name = "article_form_definition" });
                endpoints.MapControllerRoute(name: "article_form_save_data", pattern: "article/save_form_data", new { controller = "Article", action = "SaveArticleForm", page_name = "article_form_save_data" });
                endpoints.MapControllerRoute(name: "article_create_comments", pattern: "article/create_article_comments", new { controller = "Article", action = "CreateArticleComments", page_name = "article_create_comments" });

                endpoints.MapControllerRoute(name: "article_delete_conversation", pattern: "article/delete_article_conversation", new { controller = "Article", action = "DeleteArticleConversation", page_name = "article_delete_conversation" });
                endpoints.MapControllerRoute(name: "article_delete_all_conversations", pattern: "article/delete_article_conversations", new { controller = "Article", action = "DeleteArticleConversations", page_name = "article_delete_all_conversations" });

                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
