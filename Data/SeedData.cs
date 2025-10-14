using MicroJobBoard.API.Models;

namespace MicroJobBoard.API.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // Check if database is already seeded
        if (context.Users.Any())
        {
            return;
        }

        // Seed Users
        var users = new[]
        {
            new User
            {
                FullName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "John Employer",
                Email = "employer@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("employer123"),
                Role = "Employer",
                Location = "New York, NY",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "Jane Seeker",
                Email = "seeker@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("seeker123"),
                Role = "Seeker",
                Location = "San Francisco, CA",
                Bio = "Experienced software developer looking for new opportunities",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "Sarah Williams",
                Email = "sarah@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Employer",
                Location = "Austin, TX",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "Mike Johnson",
                Email = "mike@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Seeker",
                Location = "Seattle, WA",
                Bio = "Full-stack developer with 5 years of experience",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Jobs
        var jobs = new[]
        {
            new Job
            {
                Title = "Senior React Developer",
                Company = "Tech Corp",
                Location = "New York, NY",
                JobType = "Full-time",
                Category = "Software Development",
                Salary = "$120,000 - $150,000",
                Description = @"We are looking for an experienced React developer to join our team.

About the Role:
As a Senior React Developer, you will be responsible for developing and maintaining high-quality web applications using React.js and related technologies.

Responsibilities:
• Develop new user-facing features using React.js
• Build reusable components and front-end libraries
• Translate designs and wireframes into high-quality code
• Optimize components for maximum performance

Requirements:
• 5+ years of experience with React.js
• Strong proficiency in JavaScript, including ES6+
• Experience with Redux or similar state management
• Familiarity with RESTful APIs",
                Status = "Approved",
                EmployerId = 2,
                PostedDate = DateTime.UtcNow.AddDays(-5)
            },
            new Job
            {
                Title = "UX/UI Designer",
                Company = "Design Studio",
                Location = "San Francisco, CA",
                JobType = "Full-time",
                Category = "Design",
                Salary = "$90,000 - $120,000",
                Description = @"Join our creative team as a UX/UI Designer.

We are seeking a talented designer to create beautiful and intuitive user interfaces for our web and mobile applications.

Requirements:
• 3+ years of UX/UI design experience
• Proficiency in Figma, Sketch, or Adobe XD
• Strong portfolio demonstrating design skills
• Understanding of user-centered design principles",
                Status = "Approved",
                EmployerId = 2,
                PostedDate = DateTime.UtcNow.AddDays(-7)
            },
            new Job
            {
                Title = "Full Stack Developer",
                Company = "StartupXYZ",
                Location = "Remote",
                JobType = "Contract",
                Category = "Software Development",
                Salary = "$100,000 - $130,000",
                Description = @"Build scalable web applications using modern technologies.

We're looking for a full-stack developer to help us build the next generation of our platform.

Tech Stack:
• Frontend: React, TypeScript
• Backend: Node.js, Express
• Database: PostgreSQL
• Cloud: AWS",
                Status = "Approved",
                EmployerId = 4,
                PostedDate = DateTime.UtcNow.AddDays(-3)
            },
            new Job
            {
                Title = "Backend Engineer",
                Company = "Tech Startup",
                Location = "Boston, MA",
                JobType = "Full-time",
                Category = "Software Development",
                Salary = "$110,000 - $140,000",
                Description = @"Join our backend team to build robust APIs and services.

Requirements:
• Experience with .NET Core or Node.js
• Strong understanding of database design
• Knowledge of microservices architecture
• Experience with cloud platforms (Azure/AWS)",
                Status = "Pending",
                EmployerId = 4,
                PostedDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Jobs.AddRange(jobs);
        context.SaveChanges();

        // Seed Applications
        var applications = new[]
        {
            new Application
            {
                JobId = 1,
                ApplicantId = 3,
                CoverLetter = "I am very interested in this position and believe my 5 years of experience with React makes me a great fit for your team. I have worked on several large-scale applications and am passionate about creating excellent user experiences.",
                Status = "Pending",
                AppliedDate = DateTime.UtcNow.AddDays(-2)
            },
            new Application
            {
                JobId = 2,
                ApplicantId = 5,
                CoverLetter = "With my extensive background in UX/UI design and passion for creating beautiful interfaces, I would love to join your design team. My portfolio showcases my work on various web and mobile projects.",
                Status = "Accepted",
                AppliedDate = DateTime.UtcNow.AddDays(-4)
            },
            new Application
            {
                JobId = 3,
                ApplicantId = 3,
                CoverLetter = "I have been working as a full-stack developer for 5 years and am excited about the opportunity to work with your modern tech stack. I have experience with React, Node.js, and AWS.",
                Status = "Pending",
                AppliedDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Applications.AddRange(applications);
        context.SaveChanges();
    }
}
