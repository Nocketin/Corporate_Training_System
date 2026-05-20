namespace CourseService.Infrastructure.Persistence.Migrations;

/// <summary>SQL seed for demo catalog (idempotent). Applied by <see cref="SeedDemoCourses"/> migration.</summary>
internal static class CourseDemoSeedSql
{
    private const string Ts = "2026-05-20 12:00:00+00";
    private const string AuthorId = "11111111-1111-4111-8111-111111111111";

    public static readonly string Up = """
        INSERT INTO "Courses" ("Id", "CreatedAt", "UpdatedAt", "Title", "Description", "AuthorId", "Price", "CoverImageBucket", "CoverImageKey")
        VALUES
          ('a1000001-0001-4001-8001-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'Основы .NET и микросервисов', 'Практический курс для backend-разработчиков: ASP.NET Core, Clean Architecture, EF Core, REST API и паттерны устойчивых сервисов.', '{AuthorId}', 4990.00, NULL, NULL),
          ('a1000002-0002-4002-8002-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'React и TypeScript для корпоративных приложений', 'Современный фронтенд: компонентная архитектура, маршрутизация, работа с API и модульные стили SCSS.', '{AuthorId}', 3990.00, NULL, NULL),
          ('a1000003-0003-4003-8003-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'Apache Kafka и event-driven архитектура', 'Асинхронное взаимодействие сервисов: события, outbox, идемпотентные consumer и наблюдаемость.', '{AuthorId}', 5490.00, NULL, NULL),
          ('a1000004-0004-4004-8004-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'DevOps: Docker, observability и CI', 'Инфраструктура учебной платформы: compose, Consul, Redis, Seq, Prometheus и health-checks.', '{AuthorId}', 4490.00, NULL, NULL),
          ('a1000005-0005-4005-8005-000000000005', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'Лидерство и коммуникации в IT-командах', 'Soft skills для тимлидов и senior-инженеров: обратная связь, фасилитация и рост сотрудников.', '{AuthorId}', 2990.00, NULL, NULL)
        ON CONFLICT ("Id") DO NOTHING;

        INSERT INTO "Modules" ("Id", "CreatedAt", "UpdatedAt", "CourseId", "Title", "Order")
        VALUES
          ('b1000001-0001-4001-8001-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000001-0001-4001-8001-000000000001', 'Введение в платформу', 0),
          ('b1000001-0001-4001-8001-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000001-0001-4001-8001-000000000001', 'API и данные', 1),
          ('b1000002-0002-4002-8002-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000002-0002-4002-8002-000000000002', 'Фундамент', 0),
          ('b1000002-0002-4002-8002-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000002-0002-4002-8002-000000000002', 'Интерфейс продукта', 1),
          ('b1000003-0003-4003-8003-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000003-0003-4003-8003-000000000003', 'События и брокер', 0),
          ('b1000003-0003-4003-8003-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000003-0003-4003-8003-000000000003', 'Надёжность', 1),
          ('b1000004-0004-4004-8004-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000004-0004-4004-8004-000000000004', 'Контейнеры', 0),
          ('b1000004-0004-4004-8004-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000004-0004-4004-8004-000000000004', 'Наблюдаемость', 1),
          ('b1000005-0005-4005-8005-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000005-0005-4005-8005-000000000005', 'Командная работа', 0),
          ('b1000005-0005-4005-8005-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'a1000005-0005-4005-8005-000000000005', 'Развитие людей', 1)
        ON CONFLICT ("Id") DO NOTHING;

        INSERT INTO "Lessons" ("Id", "CreatedAt", "UpdatedAt", "ModuleId", "Title", "ContentUrl", "TextContent", "DurationMinutes", "Order")
        VALUES
          ('c1000001-0001-4001-8001-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000001-0001-4001-8001-000000000001', 'Что такое .NET 10 и зачем он бизнесу', NULL, 'Обзор экосистемы, LTS-релизы и типичные сценарии в корпоративной разработке.', 15, 0),
          ('c1000001-0001-4001-8001-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000001-0001-4001-8001-000000000001', 'Структура микросервисного решения', NULL, 'Слои API, Application, Domain, Infrastructure и границы ответственности.', 20, 1),
          ('c1000001-0001-4001-8001-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000001-0001-4001-8001-000000000002', 'REST и ProblemDetails', NULL, 'Единый формат ошибок, валидация и версионирование контрактов.', 25, 0),
          ('c1000001-0001-4001-8001-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000001-0001-4001-8001-000000000002', 'EF Core и Database-per-service', NULL, 'Миграции, транзакции и изоляция схем PostgreSQL.', 30, 1),
          ('c1000001-0001-4001-8001-000000000005', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000001-0001-4001-8001-000000000002', 'Практика: проектирование Course API', NULL, 'Разбор эндпоинтов каталога курсов из учебного проекта CTS.', 35, 2),
          ('c1000002-0002-4002-8002-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000002-0002-4002-8002-000000000001', 'Vite + React 18', NULL, 'Быстрый dev-сервер, HMR и структура production-сборки.', 18, 0),
          ('c1000002-0002-4002-8002-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000002-0002-4002-8002-000000000001', 'TypeScript в команде', NULL, 'Типы DTO, строгий режим и соглашения по именованию.', 22, 1),
          ('c1000002-0002-4002-8002-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000002-0002-4002-8002-000000000002', 'Маршрутизация и layout', NULL, 'Вложенные роуты, общая шапка и защищённые разделы.', 20, 0),
          ('c1000002-0002-4002-8002-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000002-0002-4002-8002-000000000002', 'Формы и валидация', NULL, 'Состояния загрузки, ошибки API и UX обратной связи.', 25, 1),
          ('c1000002-0002-4002-8002-000000000005', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000002-0002-4002-8002-000000000002', 'SCSS Modules и дизайн-токены', NULL, 'Переиспользуемые переменные, карточки и адаптивная сетка.', 28, 2),
          ('c1000003-0003-4003-8003-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000003-0003-4003-8003-000000000001', 'Зачем Kafka в микросервисах', NULL, 'Сравнение sync/async, гарантии доставки и топики.', 20, 0),
          ('c1000003-0003-4003-8003-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000003-0003-4003-8003-000000000001', 'CourseCreatedEvent end-to-end', NULL, 'Публикация из Course Service и проекция в Learning.', 30, 1),
          ('c1000003-0003-4003-8003-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000003-0003-4003-8003-000000000002', 'Transactional Outbox', NULL, 'Запись события в Postgres и фоновая отправка в Kafka (transactional outbox).', 35, 0),
          ('c1000003-0003-4003-8003-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000003-0003-4003-8003-000000000002', 'Идемпотентность consumer', NULL, 'Повторные сообщения, ключи дедупликации и тестирование.', 30, 1),
          ('c1000004-0004-4004-8004-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000004-0004-4004-8004-000000000001', 'docker-compose для CTS', NULL, 'Postgres, Kafka, MinIO, Redis и сеть сервисов.', 25, 0),
          ('c1000004-0004-4004-8004-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000004-0004-4004-8004-000000000001', 'Service Discovery с Consul', NULL, 'Регистрация сервисов и динамические маршруты YARP.', 28, 1),
          ('c1000004-0004-4004-8004-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000004-0004-4004-8004-000000000002', 'Логи в Seq', NULL, 'Структурированное логирование Serilog и корреляция запросов.', 22, 0),
          ('c1000004-0004-4004-8004-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000004-0004-4004-8004-000000000002', 'Метрики Prometheus', NULL, 'Экспорт /metrics и базовые дашборды Grafana.', 24, 1),
          ('c1000005-0005-4005-8005-000000000001', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000005-0005-4005-8005-000000000001', 'Психологическая безопасность', NULL, 'Как создавать среду для вопросов и экспериментов.', 18, 0),
          ('c1000005-0005-4005-8005-000000000002', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000005-0005-4005-8005-000000000001', 'Эффективные 1:1', NULL, 'Структура встречи, цели и фиксация договорённостей.', 20, 1),
          ('c1000005-0005-4005-8005-000000000003', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000005-0005-4005-8005-000000000002', 'Коучинг вместо микроменеджмента', NULL, 'Вопросы, делегирование и зоны ответственности.', 22, 0),
          ('c1000005-0005-4005-8005-000000000004', TIMESTAMPTZ '{Ts}', TIMESTAMPTZ '{Ts}', 'b1000005-0005-4005-8005-000000000002', 'План развития инженера', NULL, 'Матрица компетенций и измеримые цели на квартал.', 25, 1)
        ON CONFLICT ("Id") DO NOTHING;
        """.Replace("{Ts}", Ts, StringComparison.Ordinal).Replace("{AuthorId}", AuthorId, StringComparison.Ordinal);

    public static readonly string Down = """
        DELETE FROM "Courses"
        WHERE "Id" IN (
          'a1000001-0001-4001-8001-000000000001',
          'a1000002-0002-4002-8002-000000000002',
          'a1000003-0003-4003-8003-000000000003',
          'a1000004-0004-4004-8004-000000000004',
          'a1000005-0005-4005-8005-000000000005'
        );
        """;
}
