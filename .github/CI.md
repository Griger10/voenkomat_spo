# GitHub Actions

## Задание 1: Debug и Release

Workflow: `.github/workflows/build.yml`, имя в Actions — **PR build and artifacts**.

Запускается при открытии pull request, новых коммитах в нём, повторном открытии
и переводе из draft в ready. Ограничения на целевую ветку нет.
Также доступен ручной запуск через `workflow_dispatch`.

Задание `build` использует матрицу `configuration: [Debug, Release]` с двумя
параллельными заданиями. Для каждой конфигурации выполняются восстановление
пакетов, сборка решения, тесты и публикация WPF-приложения. Ошибка одной
конфигурации не отменяет проверку второй (`fail-fast: false`).

После успешных тестов загружаются два отдельных артефакта:

| Артефакт | Содержимое |
|---|---|
| `build-Debug` | Приложение Debug, библиотеки и файлы для запуска |
| `build-Release` | Приложение Release, библиотеки и файлы для запуска |

GitHub предоставляет каждый артефакт для скачивания отдельным ZIP-архивом.
Срок хранения — 14 дней. Публикация framework-dependent: на компьютере
получателя требуется .NET Desktop Runtime 10 и Windows 10 версии 2004 или новее.
Для работы приложения отдельно настраиваются `.env` и PostgreSQL;
секреты подключения и локальные PDF не включаются в артефакты.

## Задание 2: покрытие кода

Workflow: `.github/workflows/coverage.yml`, имя в Actions — **Main code coverage**.

Запускается при push в `main` и вручную. По условию задания указана именно
ветка `main`; push только в `master` этот workflow не запускает.

1. Задание `build` восстанавливает зависимости и собирает решение в Release.
2. Собранные файлы передаются во временном артефакте `release-test-binaries`
   со сроком хранения 1 день.
3. Задание `test` ожидает `build` через `needs: build`, восстанавливает зависимости,
   скачивает готовые сборки и запускает тесты без повторной компиляции.
4. `coverlet.collector` собирает покрытие с помощью `--collect "XPlat Code Coverage"`.
   Настройки находятся в `tests/coverage.runsettings`.
5. Cobertura XML публикуется в артефакте **`code-coverage`** на 14 дней.
   В Summary запуска дополнительно выводится покрытие строк и ветвей.

В отчёт включаются сборки проекта. Тестовая сборка, миграции и автоматически
сгенерированные файлы исключены. Минимальный процент покрытия не задан:
отчёт отражает фактическое покрытие существующими тестами.

Оба workflow работают на `windows-latest`, поскольку решение содержит WPF.
Устанавливается .NET SDK 10.0.x. Текущие тесты проверяют хеширование паролей
и не требуют БД, Docker, `.env` или GitHub Secrets.

## Где скачать результаты

После публикации файлов workflow в репозитории откройте **Actions**, выберите
workflow и успешный запуск. Внизу страницы запуска находится раздел **Artifacts**:
для PR там доступны `build-Debug` и `build-Release`, для push в `main` — `code-coverage`.
В архиве покрытия находится `coverage.cobertura.xml`.

Добавление YAML локально само по себе не создаёт запуск на GitHub: файлы нужно
закоммитить и отправить в репозиторий, затем открыть PR или выполнить push в `main`.
Существующий открытый PR получит запуск при следующем push в его ветку.

## Локальная проверка

Из корня репозитория на Windows:

```powershell
dotnet restore VoenkomatSpo.slnx
dotnet build VoenkomatSpo.slnx --configuration Debug --no-restore
dotnet test tests/VoenkomatSpo.Tests --configuration Debug --no-build --no-restore
dotnet build VoenkomatSpo.slnx --configuration Release --no-restore
dotnet test tests/VoenkomatSpo.Tests --configuration Release --no-build --no-restore
dotnet test tests/VoenkomatSpo.Tests --configuration Release --no-build --no-restore --collect "XPlat Code Coverage" --settings tests/coverage.runsettings --results-directory artifacts/test-results
```

## Вопросы для закрепления

**1. Какова цель использования GitHub Actions?**

Автоматизировать операции разработки при событиях в репозитории: сборку,
тестирование, проверку качества, создание артефактов и развёртывание.
В этом проекте Actions автоматически проверяет изменения и сохраняет результаты.

**2. Какую роль выполняют YAML-файлы в GitHub Actions?**

Это описание workflow в `.github/workflows`: какие события его запускают (`on`),
какие задания выполняются (`jobs`), на каких runner (`runs-on`), в каком порядке
(`needs`), с какими вариантами матрицы (`strategy.matrix`) и шагами (`steps`).

Документация: [устройство GitHub Actions](https://docs.github.com/en/actions/about-github-actions/understanding-github-actions),
[артефакты](https://github.com/actions/upload-artifact),
[покрытие .NET с Coverlet](https://learn.microsoft.com/dotnet/core/testing/unit-testing-code-coverage).
