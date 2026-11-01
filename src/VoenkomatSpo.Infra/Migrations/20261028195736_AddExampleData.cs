using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoenkomatSpo.Infra.Migrations;

public partial class AddExampleData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "City",
            columns: new[] { "id", "name" },
            values: new object[,]
            {
                { 1, "Москва" },
                { 2, "Химки" },
                { 3, "Долгопрудный" },
                { 4, "Мытищи" },
                { 5, "Зеленоград" },
            });

        migrationBuilder.InsertData(
            table: "StudyGroup",
            columns: new[] { "id", "name", "department", "graduation_year" },
            values: new object[,]
            {
                { 1, "ИС-23-1", "Информационные системы и программирование", 2027 },
                { 2, "СА-24-1", "Сетевое и системное администрирование", 2028 },
                { 3, "ЭМ-22-2", "Техническая эксплуатация электрического оборудования", 2026 },
                { 4, "С-25-1", "Связь и информационные системы", 2029 },
                { 5, "ИС-26-1", "Информационные системы и программирование", 2030 },
            });

        migrationBuilder.InsertData(
            table: "MilitaryCommissariat",
            columns: new[] { "id", "name", "address", "phone" },
            values: new object[,]
            {
                { 1, "Военный комиссариат Савёловского района (учебный)", "г. Москва, ул. Учебная, д. 12", "+7 (495) 000-00-01" },
                { 2, "Военный комиссариат городского округа Химки (учебный)", "г. Химки, ул. Примерная, д. 8", "+7 (495) 000-00-02" },
                { 3, "Военный комиссариат Мытищ и Долгопрудного (учебный)", "г. Мытищи, ул. Демонстрационная, д. 5", "+7 (495) 000-00-03" },
            });

        migrationBuilder.InsertData(
            table: "Role",
            columns: new[] { "id", "name", "access_rights" },
            values: new object[,]
            {
                { 1, "Администратор", "users.read,users.write,students.read,students.write,cards.read,cards.write,reports.read,reports.write,events.read,events.write,audit.read" },
                { 2, "Специалист воинского учета", "students.read,students.write,cards.read,cards.write,reports.read,reports.write,events.read,events.write" },
                { 3, "Сотрудник учебной части", "students.read,students.write,cards.read" },
                { 4, "Наблюдатель", "students.read,cards.read,reports.read,events.read" },
            });

        migrationBuilder.InsertData(
            table: "User",
            columns: new[] { "id", "login", "password_hash", "last_login_at", "registered_at", "role_id" },
            values: new object[,]
            {
                { 1, "admin.demo", "pbkdf2-sha256$210000$UvhEHvGA1zr0UWe2sKX1tA==$H0LumF6PFzbaSvYC3K8wu7utWyWZEFaopelvvTWCszI=", new DateTime(2026, 10, 27, 7, 45, 0, DateTimeKind.Utc), new DateTime(2025, 8, 20, 9, 0, 0, DateTimeKind.Utc), 1 },
                { 2, "ivanova.demo", "pbkdf2-sha256$210000$8uigdeRengHBUBvbh9UOZg==$pcwKQeuBlR6cI9uQuF/cTlAAn4Puc9drKr/7a8du6oc=", new DateTime(2026, 10, 27, 8, 15, 0, DateTimeKind.Utc), new DateTime(2025, 8, 25, 10, 0, 0, DateTimeKind.Utc), 2 },
                { 3, "petrov.demo", "pbkdf2-sha256$210000$vFdhF2iMU51pYMBCzgdL9Q==$DIGhn8jHg/bFmxvneXQeDhicx9RVgj3QBvmkHpaQS4o=", new DateTime(2026, 10, 26, 12, 30, 0, DateTimeKind.Utc), new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc), 2 },
                { 4, "smirnova.demo", "pbkdf2-sha256$210000$lnpkXwBM8363GRsDzvcPtQ==$K+3fkKiHyLozQyWMOYU9TKOaSw0f8A/Q1POLXrYOO0M=", new DateTime(2026, 10, 26, 9, 0, 0, DateTimeKind.Utc), new DateTime(2025, 8, 26, 9, 30, 0, DateTimeKind.Utc), 3 },
                { 5, "observer.demo", "pbkdf2-sha256$210000$wmXdD2asfS/cGgxPLkRoQA==$G36S+g6ug9bnng+Q0TXvew8JZQubTr29SQZsLbxeT0E=", null, new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Utc), 4 },
            });

        migrationBuilder.InsertData(
            table: "Student",
            columns: new[] { "id", "personal_file_number", "file_transferred_at", "last_name", "first_name", "patronymic", "birth_date", "city_id", "street", "house_number", "post_index", "passport_data", "status", "enrolled_at", "group_id" },
            values: new object[,]
            {
                { 1, "ЛД-2023-0017", null, "Соколов", "Александр", "Дмитриевич", new DateOnly(2006, 4, 12), 1, "Башиловская", "18", "127083", "ТЕСТ 0000 000001; учебные данные, документ недействителен", "Обучается", new DateOnly(2023, 9, 1), 1 },
                { 2, "ЛД-2024-0042", null, "Кузнецов", "Михаил", "Андреевич", new DateOnly(2007, 11, 3), 2, "Молодёжная", "7Б", "141410", "ТЕСТ 0000 000002; учебные данные, документ недействителен", "Обучается", new DateOnly(2024, 9, 1), 2 },
                { 3, "ЛД-2022-0008", new DateOnly(2026, 7, 10), "Попов", "Илья", "Сергеевич", new DateOnly(2005, 2, 21), 3, "Первомайская", "24", "141700", "ТЕСТ 0000 000003; учебные данные, документ недействителен", "Выпущен", new DateOnly(2022, 9, 1), 3 },
                { 4, "ЛД-2025-0091", null, "Волкова", "Анна", "Павловна", new DateOnly(2008, 6, 15), 4, "Лётная", "3А", "141018", "ТЕСТ 0000 000004; учебные данные, документ недействителен", "Обучается", new DateOnly(2025, 9, 1), 4 },
                { 5, "ЛД-2023-0056", null, "Морозов", "Даниил", "Олегович", new DateOnly(2006, 9, 28), 1, "Вятская", "11/2", "127015", "ТЕСТ 0000 000005; учебные данные, документ недействителен", "Академический отпуск", new DateOnly(2023, 9, 1), 1 },
                { 6, "ЛД-2024-0063", new DateOnly(2026, 3, 20), "Орлов", "Никита", "Алексеевич", new DateOnly(2007, 1, 17), 5, "Центральный проспект", "402", "124482", "ТЕСТ 0000 000006; учебные данные, документ недействителен", "Отчислен", new DateOnly(2024, 9, 1), 2 },
                { 7, "ЛД-2026-0004", null, "Лебедев", "Артём", "Игоревич", new DateOnly(2010, 3, 9), 2, "Победы", "9", "141400", "ТЕСТ 0000 000007; учебные данные, документ недействителен", "Обучается", new DateOnly(2026, 9, 1), 5 },
                { 8, "ЛД-2026-0015", null, "Ким", "Давид", null, new DateOnly(2009, 7, 24), 3, "Спортивная", "5", "141701", "ТЕСТ 0000 000008; учебные данные, документ недействителен", "Обучается", new DateOnly(2026, 9, 1), 5 },
                { 9, "ЛД-2023-0072", null, "Новиков", "Егор", "Максимович", new DateOnly(2006, 12, 10), 4, "Мира", "16", "141008", "ТЕСТ 0000 000009; учебные данные, документ недействителен", "Обучается", new DateOnly(2023, 9, 1), 1 },
                { 10, "ЛД-2024-0088", null, "Фёдоров", "Роман", "Викторович", new DateOnly(2007, 5, 30), 1, "Петровско-Разумовский проезд", "6", "127287", "ТЕСТ 0000 000010; учебные данные, документ недействителен", "Переведён", new DateOnly(2024, 9, 1), 2 },
            });

        migrationBuilder.InsertData(
            table: "PersonalCard",
            columns: new[] { "id", "record_category", "fitness_category", "military_specialty", "document_type", "document_number", "document_issued_at", "document_requested_at", "created_at", "is_checked", "updated_at", "is_archived", "removed_at", "student_id", "commissariat_id", "created_by" },
            values: new object[,]
            {
                { 1, "Призывник", "А", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-001", new DateOnly(2023, 4, 18), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 9, 15, 9, 0, 0, DateTimeKind.Utc), false, null, 1, 1, 2 },
                { 2, "Призывник", "Б-3", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-002", new DateOnly(2024, 4, 22), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 9, 18, 11, 0, 0, DateTimeKind.Utc), false, null, 2, 2, 2 },
                { 3, "Призывник", "Б-4", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-003", new DateOnly(2022, 5, 12), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 7, 10, 10, 0, 0, DateTimeKind.Utc), true, new DateOnly(2026, 7, 10), 3, 3, 2 },
                { 4, "Военнообязанная", "А", "901000", "Военный билет", "ТЕСТ-ВБ-004", new DateOnly(2026, 9, 10), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 9, 12, 9, 0, 0, DateTimeKind.Utc), false, null, 4, 3, 2 },
                { 5, "Призывник", "Г", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-005", new DateOnly(2023, 5, 15), null, new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 10, 6, 8, 0, 0, DateTimeKind.Utc), false, null, 5, 1, 3 },
                { 6, "Призывник", "Б-3", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-006", new DateOnly(2024, 3, 19), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 3, 20, 9, 0, 0, DateTimeKind.Utc), true, new DateOnly(2026, 3, 20), 6, 1, 2 },
                { 7, "Подлежит первоначальной постановке", null, null, "Документ не представлен", null, null, new DateOnly(2026, 9, 5), new DateTime(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc), false, null, false, null, 7, 2, 3 },
                { 8, "Подлежит первоначальной постановке", null, null, "Документ не представлен", null, null, null, new DateTime(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc), false, null, false, null, 8, 3, 3 },
                { 9, "В запасе", "В", "999000", "Военный билет", "ТЕСТ-ВБ-009", new DateOnly(2025, 6, 18), null, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 9, 20, 10, 0, 0, DateTimeKind.Utc), false, null, 9, 3, 2 },
                { 10, "Призывник", "А", null, "Удостоверение гражданина, подлежащего призыву", "ТЕСТ-ПР-010", new DateOnly(2024, 4, 8), null, new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 10, 23, 9, 0, 0, DateTimeKind.Utc), false, null, 10, 1, 3 },
            });

        migrationBuilder.InsertData(
            table: "Report",
            columns: new[] { "id", "year", "formed_at", "is_reconciled", "submitted_at", "commissariat_id", "formed_by" },
            values: new object[,]
            {
                { 1, 2025, new DateTime(2025, 11, 1, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2025, 11, 5, 10, 0, 0, DateTimeKind.Utc), 1, 2 },
                { 2, 2025, new DateTime(2025, 11, 2, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2025, 11, 6, 9, 30, 0, DateTimeKind.Utc), 2, 2 },
                { 3, 2025, new DateTime(2025, 11, 3, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2025, 11, 7, 11, 0, 0, DateTimeKind.Utc), 3, 2 },
                { 4, 2026, new DateTime(2026, 10, 20, 8, 0, 0, DateTimeKind.Utc), true, new DateTime(2026, 10, 22, 10, 0, 0, DateTimeKind.Utc), 1, 3 },
                { 5, 2026, new DateTime(2026, 10, 23, 8, 0, 0, DateTimeKind.Utc), true, null, 2, 2 },
                { 6, 2026, new DateTime(2026, 10, 26, 8, 0, 0, DateTimeKind.Utc), false, null, 3, 3 },
            });

        migrationBuilder.InsertData(
            table: "ChangeEvent",
            columns: new[] { "id", "event_type", "event_date", "notified_at", "deadline", "sent_at", "student_id", "notified_by", "commissariat_id" },
            values: new object[,]
            {
                { 1, "Выпуск", new DateOnly(2026, 6, 30), new DateTime(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 7, 14), new DateTime(2026, 7, 3, 9, 0, 0, DateTimeKind.Utc), 3, 2, 3 },
                { 2, "Отчисление", new DateOnly(2026, 3, 16), new DateTime(2026, 3, 17, 9, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 3, 30), new DateTime(2026, 3, 19, 10, 0, 0, DateTimeKind.Utc), 6, 2, 1 },
                { 3, "Академический отпуск", new DateOnly(2026, 10, 1), new DateTime(2026, 10, 2, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 10, 15), new DateTime(2026, 10, 6, 10, 0, 0, DateTimeKind.Utc), 5, 3, 1 },
                { 4, "Изменение адреса", new DateOnly(2026, 9, 14), new DateTime(2026, 9, 15, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 9, 28), new DateTime(2026, 9, 18, 10, 0, 0, DateTimeKind.Utc), 2, 2, 2 },
                { 5, "Зачисление", new DateOnly(2026, 9, 1), new DateTime(2026, 9, 2, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 9, 15), new DateTime(2026, 9, 5, 10, 0, 0, DateTimeKind.Utc), 7, 3, 2 },
                { 6, "Зачисление", new DateOnly(2026, 9, 1), new DateTime(2026, 9, 2, 9, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 9, 15), new DateTime(2026, 9, 5, 10, 0, 0, DateTimeKind.Utc), 8, 3, 3 },
                { 7, "Перевод в другую группу", new DateOnly(2026, 10, 22), new DateTime(2026, 10, 23, 9, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 11, 5), null, 10, 3, 1 },
                { 8, "Изменение семейного положения", new DateOnly(2026, 10, 25), null, new DateOnly(2026, 11, 8), null, 1, null, 1 },
                { 9, "Изменение паспортных данных", new DateOnly(2026, 10, 20), new DateTime(2026, 10, 21, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 11, 3), null, 9, 2, 3 },
                { 10, "Запрос документа воинского учета", new DateOnly(2026, 9, 5), new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 9, 19), null, 7, 3, 2 },
            });

        migrationBuilder.InsertData(
            table: "ActionLog",
            columns: new[] { "id", "action", "table_name", "record_id", "recorded_at", "user_id" },
            values: new object[,]
            {
                { 1, "Создание пользователя", "User", 2, new DateTime(2025, 8, 25, 10, 0, 0, DateTimeKind.Utc), 1 },
                { 2, "Создание пользователя", "User", 4, new DateTime(2025, 8, 26, 9, 30, 0, DateTimeKind.Utc), 1 },
                { 3, "Создание карточки", "PersonalCard", 1, new DateTime(2025, 9, 1, 8, 0, 0, DateTimeKind.Utc), 2 },
                { 4, "Формирование отчета", "Report", 1, new DateTime(2025, 11, 1, 8, 0, 0, DateTimeKind.Utc), 2 },
                { 5, "Отправка отчета", "Report", 1, new DateTime(2025, 11, 5, 10, 0, 0, DateTimeKind.Utc), 2 },
                { 6, "Создание пользователя", "User", 3, new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc), 1 },
                { 7, "Архивирование карточки", "PersonalCard", 6, new DateTime(2026, 3, 20, 9, 0, 0, DateTimeKind.Utc), 2 },
                { 8, "Передача личного дела", "Student", 3, new DateTime(2026, 7, 10, 9, 0, 0, DateTimeKind.Utc), 4 },
                { 9, "Архивирование карточки", "PersonalCard", 3, new DateTime(2026, 7, 10, 10, 0, 0, DateTimeKind.Utc), 2 },
                { 10, "Создание студента", "Student", 7, new DateTime(2026, 9, 1, 7, 30, 0, DateTimeKind.Utc), 4 },
                { 11, "Запрос документа", "PersonalCard", 7, new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc), 3 },
                { 12, "Проверка карточки", "PersonalCard", 9, new DateTime(2026, 9, 20, 10, 0, 0, DateTimeKind.Utc), 2 },
                { 13, "Отправка отчета", "Report", 4, new DateTime(2026, 10, 22, 10, 0, 0, DateTimeKind.Utc), 3 },
                { 14, "Формирование отчета", "Report", 6, new DateTime(2026, 10, 26, 8, 0, 0, DateTimeKind.Utc), 3 },
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "ActionLog",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });

        migrationBuilder.DeleteData(
            table: "ChangeEvent",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

        migrationBuilder.DeleteData(
            table: "Report",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6 });

        migrationBuilder.DeleteData(
            table: "PersonalCard",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

        migrationBuilder.DeleteData(
            table: "Student",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

        migrationBuilder.DeleteData(
            table: "User",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5 });

        migrationBuilder.DeleteData(
            table: "Role",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4 });

        migrationBuilder.DeleteData(
            table: "MilitaryCommissariat",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3 });

        migrationBuilder.DeleteData(
            table: "StudyGroup",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5 });

        migrationBuilder.DeleteData(
            table: "City",
            keyColumn: "id",
            keyValues: new object[] { 1, 2, 3, 4, 5 });
    }
}
