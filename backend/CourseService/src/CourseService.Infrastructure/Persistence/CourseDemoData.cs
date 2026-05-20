using CourseService.Domain.Entities;

namespace CourseService.Infrastructure.Persistence;

/// <summary>Fixed demo catalog IDs and content — applied via EF migration and optional dev seeder.</summary>
public static class CourseDemoData
{
    public static readonly Guid DemoAuthorId = Guid.Parse("11111111-1111-4111-8111-111111111111");

    public static readonly DateTime SeedTimestamp = new(2026, 5, 20, 12, 0, 0, DateTimeKind.Utc);

    public static readonly Guid Course1Id = Guid.Parse("a1000001-0001-4001-8001-000000000001");
    public static readonly Guid Course2Id = Guid.Parse("a1000002-0002-4002-8002-000000000002");
    public static readonly Guid Course3Id = Guid.Parse("a1000003-0003-4003-8003-000000000003");
    public static readonly Guid Course4Id = Guid.Parse("a1000004-0004-4004-8004-000000000004");
    public static readonly Guid Course5Id = Guid.Parse("a1000005-0005-4005-8005-000000000005");

    public static IReadOnlyList<Guid> AllCourseIds { get; } =
    [
        Course1Id, Course2Id, Course3Id, Course4Id, Course5Id
    ];

    public static Course[] BuildCourses()
    {
        return
        [
            BuildCourse(
                Course1Id,
                "Основы .NET и микросервисов",
                "Практический курс для backend-разработчиков: ASP.NET Core, Clean Architecture, EF Core, REST API и паттерны устойчивых сервисов.",
                4990m,
                [
                    (Guid.Parse("b1000001-0001-4001-8001-000000000001"), "Введение в платформу", 0,
                    [
                        (Guid.Parse("c1000001-0001-4001-8001-000000000001"), "Что такое .NET 10 и зачем он бизнесу", "Обзор экосистемы, LTS-релизы и типичные сценарии в корпоративной разработке.", 15, 0),
                        (Guid.Parse("c1000001-0001-4001-8001-000000000002"), "Структура микросервисного решения", "Слои API, Application, Domain, Infrastructure и границы ответственности.", 20, 1),
                    ]),
                    (Guid.Parse("b1000001-0001-4001-8001-000000000002"), "API и данные", 1,
                    [
                        (Guid.Parse("c1000001-0001-4001-8001-000000000003"), "REST и ProblemDetails", "Единый формат ошибок, валидация и версионирование контрактов.", 25, 0),
                        (Guid.Parse("c1000001-0001-4001-8001-000000000004"), "EF Core и Database-per-service", "Миграции, транзакции и изоляция схем PostgreSQL.", 30, 1),
                        (Guid.Parse("c1000001-0001-4001-8001-000000000005"), "Практика: проектирование Course API", "Разбор эндпоинтов каталога курсов из учебного проекта CTS.", 35, 2),
                    ]),
                ]),
            BuildCourse(
                Course2Id,
                "React и TypeScript для корпоративных приложений",
                "Современный фронтенд: компонентная архитектура, маршрутизация, работа с API и модульные стили SCSS.",
                3990m,
                [
                    (Guid.Parse("b1000002-0002-4002-8002-000000000001"), "Фундамент", 0,
                    [
                        (Guid.Parse("c1000002-0002-4002-8002-000000000001"), "Vite + React 18", "Быстрый dev-сервер, HMR и структура production-сборки.", 18, 0),
                        (Guid.Parse("c1000002-0002-4002-8002-000000000002"), "TypeScript в команде", "Типы DTO, строгий режим и соглашения по именованию.", 22, 1),
                    ]),
                    (Guid.Parse("b1000002-0002-4002-8002-000000000002"), "Интерфейс продукта", 1,
                    [
                        (Guid.Parse("c1000002-0002-4002-8002-000000000003"), "Маршрутизация и layout", "Вложенные роуты, общая шапка и защищённые разделы.", 20, 0),
                        (Guid.Parse("c1000002-0002-4002-8002-000000000004"), "Формы и валидация", "Состояния загрузки, ошибки API и UX обратной связи.", 25, 1),
                        (Guid.Parse("c1000002-0002-4002-8002-000000000005"), "SCSS Modules и дизайн-токены", "Переиспользуемые переменные, карточки и адаптивная сетка.", 28, 2),
                    ]),
                ]),
            BuildCourse(
                Course3Id,
                "Apache Kafka и event-driven архитектура",
                "Асинхронное взаимодействие сервисов: события, outbox, идемпотентные consumer и наблюдаемость.",
                5490m,
                [
                    (Guid.Parse("b1000003-0003-4003-8003-000000000001"), "События и брокер", 0,
                    [
                        (Guid.Parse("c1000003-0003-4003-8003-000000000001"), "Зачем Kafka в микросервисах", "Сравнение sync/async, гарантии доставки и топики.", 20, 0),
                        (Guid.Parse("c1000003-0003-4003-8003-000000000002"), "CourseCreatedEvent end-to-end", "Публикация из Course Service и проекция в Learning.", 30, 1),
                    ]),
                    (Guid.Parse("b1000003-0003-4003-8003-000000000002"), "Надёжность", 1,
                    [
                        (Guid.Parse("c1000003-0003-4003-8003-000000000003"), "Transactional Outbox", "Запись события в Postgres и фоновая отправка в Kafka (transactional outbox).", 35, 0),
                        (Guid.Parse("c1000003-0003-4003-8003-000000000004"), "Идемпотентность consumer", "Повторные сообщения, ключи дедупликации и тестирование.", 30, 1),
                    ]),
                ]),
            BuildCourse(
                Course4Id,
                "DevOps: Docker, observability и CI",
                "Инфраструктура учебной платформы: compose, Consul, Redis, Seq, Prometheus и health-checks.",
                4490m,
                [
                    (Guid.Parse("b1000004-0004-4004-8004-000000000001"), "Контейнеры", 0,
                    [
                        (Guid.Parse("c1000004-0004-4004-8004-000000000001"), "docker-compose для CTS", "Postgres, Kafka, MinIO, Redis и сеть сервисов.", 25, 0),
                        (Guid.Parse("c1000004-0004-4004-8004-000000000002"), "Service Discovery с Consul", "Регистрация сервисов и динамические маршруты YARP.", 28, 1),
                    ]),
                    (Guid.Parse("b1000004-0004-4004-8004-000000000002"), "Наблюдаемость", 1,
                    [
                        (Guid.Parse("c1000004-0004-4004-8004-000000000003"), "Логи в Seq", "Структурированное логирование Serilog и корреляция запросов.", 22, 0),
                        (Guid.Parse("c1000004-0004-4004-8004-000000000004"), "Метрики Prometheus", "Экспорт /metrics и базовые дашборды Grafana.", 24, 1),
                    ]),
                ]),
            BuildCourse(
                Course5Id,
                "Лидерство и коммуникации в IT-командах",
                "Soft skills для тимлидов и senior-инженеров: обратная связь, фасилитация и рост сотрудников.",
                2990m,
                [
                    (Guid.Parse("b1000005-0005-4005-8005-000000000001"), "Командная работа", 0,
                    [
                        (Guid.Parse("c1000005-0005-4005-8005-000000000001"), "Психологическая безопасность", "Как создавать среду для вопросов и экспериментов.", 18, 0),
                        (Guid.Parse("c1000005-0005-4005-8005-000000000002"), "Эффективные 1:1", "Структура встречи, цели и фиксация договорённостей.", 20, 1),
                    ]),
                    (Guid.Parse("b1000005-0005-4005-8005-000000000002"), "Развитие людей", 1,
                    [
                        (Guid.Parse("c1000005-0005-4005-8005-000000000003"), "Коучинг вместо микроменеджмента", "Вопросы, делегирование и зоны ответственности.", 22, 0),
                        (Guid.Parse("c1000005-0005-4005-8005-000000000004"), "План развития инженера", "Матрица компетенций и измеримые цели на квартал.", 25, 1),
                    ]),
                ]),
        ];
    }

    private static Course BuildCourse(
        Guid courseId,
        string title,
        string description,
        decimal price,
        (Guid moduleId, string moduleTitle, int moduleOrder, (Guid lessonId, string lessonTitle, string text, int minutes, int lessonOrder)[] lessons)[] modules)
    {
        var ts = SeedTimestamp;
        var course = new Course
        {
            Title = title,
            Description = description,
            AuthorId = DemoAuthorId,
            Price = price,
        };
        course.AssignIdentity(courseId, ts);

        foreach (var (moduleId, moduleTitle, moduleOrder, lessons) in modules)
        {
            var module = new Module
            {
                Title = moduleTitle,
                Order = moduleOrder,
                Course = course,
            };
            module.AssignIdentity(moduleId, ts);

            foreach (var (lessonId, lessonTitle, text, minutes, lessonOrder) in lessons)
            {
                var lesson = new Lesson
                {
                    Title = lessonTitle,
                    TextContent = text,
                    ContentUrl = null,
                    DurationMinutes = minutes,
                    Order = lessonOrder,
                    Module = module,
                };
                lesson.AssignIdentity(lessonId, ts);
                module.Lessons.Add(lesson);
            }

            course.Modules.Add(module);
        }

        return course;
    }
}
