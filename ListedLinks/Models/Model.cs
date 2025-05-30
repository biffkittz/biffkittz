using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ListedLinks.Models
{
    public class ListedLinksContext : DbContext
    {
        public DbSet<ListedLink> ListedLinks { get; set; }
        public DbSet<IPAddressString> IPAddressStrings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Book> Books { get; set; }

        public string DbPath { get; }

        public static string? ConnectionString { get; set; } = string.Empty;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ListedLinksContext.ConnectionString /*$"Filename=ListedLinks.db"*/);
            optionsBuilder.UseSeeding((context, _) =>
            {
                var seedingGenreComplete = context.Set<Genre>().Any();
                if (!seedingGenreComplete)
                {
                    var programmingGenre = new Genre
                    {
                        Name = "Programming",
                        Description = "Books that cover programming concepts and techniques."
                    };

                    var horrorGenre = new Genre
                    {
                        Name = "Horror",
                        Description = "Scary stuff."
                    };

                    context.AddRange(programmingGenre, horrorGenre);

                    context.AddRange(
                        new Book
                        {
                            Title = "The Pragmatic Programmer: Your Journey To Mastery",
                            Author = "Andrew Hunt, David Thomas",
                            Blurb = "A guide to becoming a better programmer, covering topics such as code organization, debugging, and design patterns.",
                            Genre = programmingGenre,
                        },
                        new Book
                        {
                            Title = "Horror",
                            Author = "Mr H",
                            Blurb = "Scary stuff.",
                            Genre = horrorGenre,
                        }
                    );
                }

                var seedingListedLinkComplete = context.Set<ListedLink>().Any(lg => lg.Title == "Chapter 1: The Game We Didn’t Know We Were Playing");
                if (!seedingListedLinkComplete)
                {
                    context.AddRange(
                        new ListedLink
                        {
                            Title = "Setting Up an On-Premises Rancher Cluster with k3s, Helm and Hyper-V Manager",
                            Url = "https://medium.com/@Html.Raw(\"@\")saadullahkhanwarsi/title-setting-up-an-on-premise-k3s-cluster-with-rancher-helm-and-hyper-v-manager-cc888edb178c",
                            Author = "Saad Ullah Khan Warsi, medium.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "A sample showing how to use client results in SignalR to build a turn based game",
                            Url = "https://github.com/davidfowl/TriviaR",
                            Author = "github.com/davidfowl",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "RabbitMQ Tutorials",
                            Url = "https://www.rabbitmq.com/tutorials",
                            Author = "rabbitmq.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Chapter 1: The Game We Didn’t Know We Were Playing",
                            Url = "https://codewithshadman.com/the-game-we-didnt-know-we-were-playing/",
                            Author = "Shadman Kudchikar, codewithshadman.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Design Patterns",
                            Url = "https://refactoring.guru/design-patterns",
                            Author = "refactoring.guru",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "ASP.NET MVC with React components",
                            Url = "https://reactjs.net/tutorials/aspnetcore.html",
                            Author = "reactjs.net/tutorials",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Psycopg2.pool crashes when the thread pool runs out",
                            Url = "https://stackoverflow.com/questions/64603192/psycopg2-pool-crashes-when-the-thread-pool-runs-out",
                            Author = "stackoverflow.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Bank-api repo",
                            Url = "https://github.com/erwinkramer/bank-api?tab=readme-ov-file#bank-api",
                            Author = "github.com/erwinkramer",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Learn go",
                            Url = "https://go.dev/learn/",
                            Author = "go.dev",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "How to Set Up an NFS Mount on Ubuntu (Step-by-Step Guide)",
                            Url = "https://www.digitalocean.com/community/tutorials/how-to-set-up-an-nfs-mount-on-ubuntu-20-04/",
                            Author = "Brian Boucheron and Vinayak Baranwal, digitalocean.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Deep Dive: AddController vs AddMvc vs AddControllersWithViews vs ... in ASP.NET Core",
                            Url = "https://www.linkedin.com/pulse/deep-dive-addcontroller-vs-addmvc-addrazorpages-aspnet-sandeep-pal-omvzc/",
                            Author = "Sandeep Pal, linkedin.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Create a Kubernetes Cluster using Virtualbox — The Hard Way",
                            Url = "https://medium.com/@Html.Raw(\"@\")mojabi.rafi/create-a-kubernetes-cluster-using-virtualbox-and-without-vagrant-90a14d791617",
                            Author = "Mojabi Rafi, medium.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Python_koans",
                            Url = "https://github.com/gregmalcolm/python_koans",
                            Author = "Greg Malcolm, github.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Using the ChatGPT API in C# and .NET",
                            Url = "https://medium.com/@Html.Raw(\"@\")msgold/using-the-chatgpt-api-in-c-and-net-8855eca270e2",
                            Author = "Michael Gold, medium.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Your First Component",
                            Url = "https://react.dev/learn/your-first-component",
                            Author = "react.dev",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Architect Modern Web Applications with ASP.NET Core and Azure",
                            Url = "https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/?WT.mc_id=dotnet-35129-website",
                            Author = "learn.microsoft.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "PostgreSQL Tutorial",
                            Url = "https://www.postgresql.org/docs/current/tutorial.html",
                            Author = "postgresql.org",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "Shadcn/ui is a set of beautifully-designed, accessible components",
                            Url = "https://ui.shadcn.com/docs",
                            Author = "ui.shadcn.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "K8s ReplicaSet",
                            Url = "https://kubernetes.io/docs/concepts/workloads/controllers/replicaset/",
                            Author = "kubernetes.io",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "React.AspNet with webpack and ChakraCore",
                            Url = "https://github.com/reactjs/React.NET/blob/main/src/React.Template/reactnet-webpack/Startup.cs",
                            Author = "github.com/reactjs/React.NET",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "What is Azure Service Bus?",
                            Url = "https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview",
                            Author = "learn.microsoft.com",
                            CreatedAt = DateTime.UtcNow
                        },
                        new ListedLink
                        {
                            Title = "SetUserFTA: UserChoice Hash defeated",
                            Url = "https://kolbi.cz/blog/2017/10/25/setuserfta-userchoice-hash-defeated-set-file-type-associations-per-user/",
                            Author = "Christoph Kolbicz",
                            CreatedAt = DateTime.UtcNow
                        }
                    );
                }

                context.SaveChanges();
            });
        }   

        //public ListedLinksContext()
        //{
        //    var folder = Environment.SpecialFolder.LocalApplicationData;
        //    var path = Environment.GetFolderPath(folder);
        //    DbPath = System.IO.Path.Join(path, "listedlinks.db");
        //}
    }

    public class ListedLinksContextSettings
    {
        public string? ConnectionString { get; set; }
    }

    public class ListedLink
    {
        public int ListedLinkId { get; set; }
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class IPAddressString
    {
        //[Required]
        //[RegularExpression(@"^(\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "Invalid IP address format.")]
        [Key]
        public string? IPAddress { get; set; }
    }

    [PrimaryKey(nameof(Text), nameof(CreatedAt))]
    public class Comment
    {
        public string? Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    [PrimaryKey(nameof(Title), nameof(Author))]
    public class Book
    {
        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Blurb { get; set; }

        public Genre? Genre { get; set; }
    }

    [PrimaryKey(nameof(Name))]
    public class Genre
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Book?>? Books { get; }
    }
}
