using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Enums;
using EducationalInstitution.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Inicializa la base de datos aplicando migraciones y datos semilla
        /// Solo ejecuta lo que no esté hecho previamente
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EducationalContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<EducationalContext>>();

            try
            {
                logger.LogInformation("Iniciando verificación de base de datos...");

                // 1. Crear base de datos si no existe
                await EnsureDatabaseCreatedAsync(context, logger);

                // 2. Aplicar migraciones pendientes
                await ApplyPendingMigrationsAsync(context, logger);

                // 3. Insertar datos semilla si no existen
                await SeedDataAsync(context, logger);

                logger.LogInformation("Base de datos inicializada correctamente");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al inicializar la base de datos: {Message}", ex.Message);
                throw;
            }
        }

        private static async Task EnsureDatabaseCreatedAsync(EducationalContext context, ILogger logger)
        {
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogInformation("Creando base de datos...");
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Base de datos creada");
            }
            else
            {
                logger.LogInformation("Base de datos ya existe");
            }
        }

        private static async Task ApplyPendingMigrationsAsync(EducationalContext context, ILogger logger)
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migraciones pendientes...", pendingMigrations.Count());

                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("  - {Migration}", migration);
                }

                await context.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas correctamente");
            }
            else
            {
                logger.LogInformation("No hay migraciones pendientes");
            }
        }

        private static async Task SeedDataAsync(EducationalContext context, ILogger logger)
        {
            logger.LogInformation("Verificando datos semilla...");

            // Verificar y crear usuario administrador
            await SeedAdminUserAsync(context, logger);

            // Verificar y crear profesores
            await SeedProfessorsAsync(context, logger);

            // Verificar y crear estudiantes (mínimo 25)
            await SeedStudentsAsync(context, logger);

            // Verificar y crear cursos
            await SeedCoursesAsync(context, logger);

            // Verificar y crear calificaciones de ejemplo
            await SeedQualificationsAsync(context, logger);

            await context.SaveChangesAsync();
            logger.LogInformation("Datos semilla verificados/insertados");
        }

        private static async Task SeedAdminUserAsync(EducationalContext context, ILogger logger)
        {
            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Creando usuario administrador...");

                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "System",
                    Email = "admin@educational.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
                logger.LogInformation("Usuario administrador creado");
            }
            else
            {
                logger.LogInformation("Usuario administrador ya existe");
            }
        }

        private static async Task SeedProfessorsAsync(EducationalContext context, ILogger logger)
        {
            var professorCount = await context.Professors.CountAsync();

            if (professorCount < 10)
            {
                logger.LogInformation("Creando profesores de ejemplo...");

                var professors = new List<Professor>();

                for (int i = professorCount + 1; i <= 10; i++)
                {
                    professors.Add(new Professor
                    {
                        EmployeeCode = $"PROF{i:D3}",
                        FirstName = $"Professor{i}",
                        LastName = $"LastName{i}",
                        Email = $"professor{i}@educational.com",
                        Phone = $"555-{2000 + i}",
                        Department = GetDepartment(i),
                        Specialization = GetSpecialization(i),
                        HireDate = DateTime.UtcNow.AddYears(-i),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Professors.AddRange(professors);
                await context.SaveChangesAsync();
                logger.LogInformation("{Count} profesores creados", professors.Count);
            }
            else
            {
                logger.LogInformation("Profesores ya existen ({Count})", professorCount);
            }
        }

        // Crea mínimo 25 estudiantes si no existen
        private static async Task SeedStudentsAsync(EducationalContext context, ILogger logger)
        {
            var studentCount = await context.Students.CountAsync();

            if (studentCount < 25)
            {
                logger.LogInformation("Creando estudiantes de ejemplo (mínimo 25)...");

                var students = new List<Student>();

                for (int i = studentCount + 1; i <= 30; i++)
                {
                    students.Add(new Student
                    {
                        StudentCode = $"EST{i:D3}",
                        FirstName = GetFirstName(i),
                        LastName = GetLastName(i),
                        Email = $"student{i}@educational.com",
                        Phone = $"555-{1000 + i}",
                        EnrollmentDate = DateTime.UtcNow.AddDays(-i * 15),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                context.Students.AddRange(students);
                await context.SaveChangesAsync();
                logger.LogInformation("{Count} estudiantes creados", students.Count);
            }
            else
            {
                logger.LogInformation("Estudiantes ya existen ({Count})", studentCount);
            }
        }

        private static async Task SeedCoursesAsync(EducationalContext context, ILogger logger)
        {
            var courseCount = await context.Courses.CountAsync();

            if (courseCount < 15)
            {
                logger.LogInformation("Creando cursos de ejemplo...");

                var courses = new List<Course>();
                var professorIds = await context.Professors.Select(p => p.Id).ToListAsync();

                if (professorIds.Any())
                {
                    for (int i = courseCount + 1; i <= 15; i++)
                    {
                        courses.Add(new Course
                        {
                            CourseCode = $"COURSE{i:D3}",
                            Name = GetCourseName(i),
                            Description = $"Comprehensive course covering {GetCourseName(i)} fundamentals and advanced topics",
                            Credits = (i % 5) + 1,
                            ProfessorId = professorIds[i % professorIds.Count],
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    context.Courses.AddRange(courses);
                    await context.SaveChangesAsync();
                    logger.LogInformation("{Count} cursos creados", courses.Count);
                }
            }
            else
            {
                logger.LogInformation("Cursos ya existen ({Count})", courseCount);
            }
        }

        private static async Task SeedQualificationsAsync(EducationalContext context, ILogger logger)
        {
            var qualificationCount = await context.Qualifications.CountAsync();

            if (qualificationCount < 50)
            {
                logger.LogInformation("Creando calificaciones de ejemplo...");

                var studentIds = await context.Students.Take(20).Select(s => s.Id).ToListAsync();
                var courseIds = await context.Courses.Take(10).Select(c => c.Id).ToListAsync();

                var qualifications = new List<Qualification>();
                var random = new Random();

                foreach (var studentId in studentIds)
                {
                    // Cada estudiante tiene calificaciones en 3-5 cursos aleatorios
                    var coursesForStudent = courseIds.OrderBy(x => random.Next()).Take(random.Next(3, 6));

                    foreach (var courseId in coursesForStudent)
                    {
                        // Verificar que no exista ya esta combinación
                        var exists = await context.Qualifications
                            .AnyAsync(q => q.StudentId == studentId && q.CourseId == courseId);

                        if (!exists)
                        {
                            qualifications.Add(new Qualification
                            {
                                StudentId = studentId,
                                CourseId = courseId,
                                Grade = Math.Round((decimal)(random.NextDouble() * 4 + 1), 2), // 1.0 - 5.0
                                QualificationDate = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                                Comments = GetRandomComment(random),
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                if (qualifications.Any())
                {
                    context.Qualifications.AddRange(qualifications);
                    await context.SaveChangesAsync();
                    logger.LogInformation("{Count} calificaciones creadas", qualifications.Count);
                }
            }
            else
            {
                logger.LogInformation("Calificaciones ya existen ({Count})", qualificationCount);
            }
        }

        // Métodos auxiliares para generar datos variados
        private static string GetDepartment(int index) => index switch
        {
            1 or 6 => "Matemáticas",
            2 or 7 => "Ciencias de la Computación",
            3 or 8 => "Física",
            4 or 9 => "Química",
            _ => "Ingeniería"
        };

        private static string GetSpecialization(int index) => index switch
        {
            1 => "Álgebra y Cálculo",
            2 => "Ingeniería de Software",
            3 => "Física Cuántica",
            4 => "Química Orgánica",
            5 => "Ingeniería Mecánica",
            6 => "Estadística",
            7 => "Sistemas de Bases de Datos",
            8 => "Física Nuclear",
            9 => "Bioquímica",
            _ => "Ingeniería General"
        };

        private static string GetFirstName(int index) => index switch
        {
            var i when i % 10 == 1 => "Juan",
            var i when i % 10 == 2 => "María",
            var i when i % 10 == 3 => "Carlos",
            var i when i % 10 == 4 => "Ana",
            var i when i % 10 == 5 => "Luis",
            var i when i % 10 == 6 => "Carmen",
            var i when i % 10 == 7 => "Pedro",
            var i when i % 10 == 8 => "Sofía",
            var i when i % 10 == 9 => "Miguel",
            _ => "Isabel"
        };

        private static string GetLastName(int index) => index switch
        {
            var i when i % 8 == 1 => "García",
            var i when i % 8 == 2 => "Rodríguez",
            var i when i % 8 == 3 => "Martínez",
            var i when i % 8 == 4 => "López",
            var i when i % 8 == 5 => "González",
            var i when i % 8 == 6 => "Pérez",
            var i when i % 8 == 7 => "Sánchez",
            _ => "Ramírez"
        };

        private static string GetCourseName(int index) => index switch
        {
            1 => "Cálculo I",
            2 => "Fundamentos de Programación",
            3 => "Física I",
            4 => "Química General",
            5 => "Álgebra Lineal",
            6 => "Estructuras de Datos",
            7 => "Física II",
            8 => "Química Orgánica",
            9 => "Estadística",
            10 => "Diseño de Bases de Datos",
            11 => "Mecánica Cuántica",
            12 => "Bioquímica",
            13 => "Cálculo II",
            14 => "Ingeniería de Software",
            _ => "Temas Avanzados"
        };

        private static string GetRandomComment(Random random) => random.Next(5) switch
        {
            0 => "Excelente desempeño durante el curso",
            1 => "Buen entendimiento de la materia",
            2 => "Necesita mejorar en algunas áreas",
            3 => "Trabajo y participación sobresalientes",
            _ => "Cumplimiento satisfactorio de los requisitos"
        };
    }
}
