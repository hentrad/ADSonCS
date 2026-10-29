namespace HashLab;
public static class Ui
{
    public const string Html = """
<!doctype html>
<html lang="ru">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Хеш-таблица — вариант 11</title>

    <style>
        /* ---------- Цвета (светлая тема по умолчанию) ---------- */
        :root {
            --bg:   #ffffff10;
                --fg:   #e6e9f0;
                --mut:  #ffffff;
                --card: #00000070;
                --bd:   #ffffff10;
                --acc:  #ff9000;
                --ok:   #4cc582;
                --bad:  #ff7b70;
                --hit:  #20202070;
        }

        /* ---------- Цвета для тёмной темы системы ---------- */
        @media (prefers-color-scheme: dark) {
            :root {
                --bg:   #ffffff10;
                --fg:   #e6e9f0;
                --mut:  #ffffff;
                --card: #00000070;
                --bd:   #ffffff10;
                --acc:  #ff9000;
                --ok:   #4cc582;
                --bad:  #ff7b70;
                --hit:  #20202070;
                --hv:   #ffffff20;
            }
        }

        /* ---------- Общая раскладка ---------- */
        body {
            margin: 0;
            font: 15px system-ui, sans-serif;
            background: var(--bg);
            color: var(--fg);
        }

        main {
            max-width: 1100px;
            margin: 0 auto;
            padding: 16px;
            display: grid;
            gap: 16px;
            grid-template-columns: minmax(300px, 380px) 1fr;
        }

        @media (max-width: 800px) {
            main { grid-template-columns: 1fr; }
        }

        h1 {
            font-size: 20px;
            margin: 0;
            grid-column: 1 / -1;
        }

        /* ---------- Карточки и элементы формы ---------- */
        .card {
            background: var(--card);
            border-radius: 10px;
            padding: 25px;
        }

        label {
            display: block;
            margin: 6px 0 2px;
            color: var(--mut);
            font-size: 13px;
        }

        input {
            width: 100%;
            box-sizing: border-box;
            padding: 8px;
            border: 1px solid var(--bd);
            border-radius: 6px;
            background: var(--bg);
            color: var(--fg);
            outline: none;
            box-shadow: none;
          }

        input:focus {
           outline: none;
            box-shadow: none;
            border-color: var(--acc);
        }

        input:hover {
            background-color: var(--hv);
        }

        button {
            margin: 8px 6px 0 0;
            padding: 8px 14px;
            border: 0;
            border-radius: 6px;
            background: var(--acc);
            color: #000000;
            cursor: pointer;
        }

        button:hover {
          background-color: #ffaf37;
        }

        button.sec {
          background: var(--bd);
          color: var(--fg);
        }

        button.sec:hover {
          background-color: var(--hv);
        }

        /* ---------- Журнал ---------- */
        #log {
            height: 260px;
            overflow: auto;
            white-space: pre-wrap;
            font: 13px ui-monospace, monospace;
        }

        .ok  { color: var(--ok); }
        .bad { color: var(--bad); }

        /* ---------- Сетка таблицы (100 ячеек) ---------- */
        #grid {
            display: grid;
            grid-template-columns: repeat(10, 1fr);
            gap: 3px;
        }

        .cell {
            border: 1px solid var(--bd);
            border-radius: 4px;
            padding: 2px 3px;
            min-height: 34px;
            font-size: 11px;
            overflow: hidden;
            background: var(--bg);
        }

        .cell b {
            display: block;
            color: var(--mut);
            font-weight: 400;
        }

        .cell.full { border-color: var(--acc); }
        .cell.hit  { background: var(--hit); }
    </style>
</head>

<body>
<main>
    <h1>Хеш-таблица вариант 11 ХФ "Л", открытое перемешивание, квадратичные пробы</h1>
    <div>
        <div class="card">
            <label>Ключ (минимум 2 символа)</label>
            <input id="key" autocomplete="off">

            <label>Информационное поле</label>
            <input id="info" autocomplete="off">

            <button onclick="add()">Занести</button>
            <button onclick="find()">Найти</button>
            <button class="sec" onclick="clr()">Очистить</button>

            <p id="stat" style="color: var(--mut); font-size: 13px"></p>
        </div>

        <div class="card" style="margin-top: 16px">
            <b>Ключи, вызвавшие коллизии</b>
            <div id="coll">—</div>
        </div>
        <div class="card" style="margin-top: 16px">
            <b>Журнал</b>
            <div id="log"></div>
        </div>
    </div>

    <div>
        <div class="card">
            <div id="grid"></div>
        </div>
    </div>
</main>

<script>
    const $ = id => document.getElementById(id);

    // Отправка POST-запроса на сервер, ответ — JSON.
    async function api(path, body) {
        const response = await fetch(path, {
            method: 'POST',
            body: JSON.stringify(body || {})
        });
        return response.json();
    }

    // Добавляет строку в начало журнала.
    function log(text, cssClass) {
        const line = document.createElement('div');
        if (cssClass) line.className = cssClass;
        line.textContent = text;
        $('log').prepend(line);
    }

    // Текст про ключ в позиции: «(пусто)» или 'ключ'.
    function keyText(key) {
        return key === null ? '(пусто)' : "'" + key + "'";
    }

    // Выводит в журнал результат операции (занесение или поиск).
    function report(title, result) {
        log('— ' + title, '');

        if (result.primary) {
            log('Первичный индекс: ' + result.primary);
        }

        result.steps.forEach((step, j) => {
            if (j > 0) {
                log('  вторичный индекс (j=' + j + '): ' + step.index + ', ключ: ' + keyText(step.key));
            } else {
                log('  позиция ' + step.index + ', ключ: ' + keyText(step.key));
            }
        });

        if (result.info != null) {
            log('Информационное поле: ' + result.info);
        }

        log(result.message, result.ok ? 'ok' : 'bad');

        // Перерисовать таблицу и подсветить просмотренные позиции.
        refreshState(result.steps.map(step => step.index));
    }

    // Занесение записи.
    async function add() {
        const result = await api('/api/add', {
            key: $('key').value,
            info: $('info').value
        });
        report('Занесение «' + $('key').value + '»', result);
    }

    // Поиск записи.
    async function find() {
        const result = await api('/api/find', { key: $('key').value });
        report('Поиск «' + $('key').value + '»', result);
    }

    // Очистка таблицы и журнала.
    async function clr() {
        await api('/api/clear');
        $('log').textContent = '';
        refreshState([]);
    }

    async function refreshState(highlighted) {
        const state = await api('/api/state');

        $('stat').textContent =
            'Занято: ' + state.count + ' из ' + state.n + ' · mod в ХФ: ' + state.m;

        const grid = $('grid');
        grid.textContent = '';

        state.cells.forEach(cell => {
            const div = document.createElement('div');

            div.className = 'cell'
                + (cell.key !== null ? ' full' : '')
                + (highlighted && highlighted.includes(cell.i) ? ' hit' : '');
            div.title = cell.key !== null ? 'info: ' + cell.info : '';

            const number = document.createElement('b');
            number.textContent = cell.i;

            div.append(number, cell.key || '');
            grid.append(div);
        });

        $('coll').textContent = state.collisions.length
            ? state.collisions.map(c => c.key + ' (iперв=' + c.primary + ')').join(', ')
            : '—';
    }

    // Enter в полях ввода = «Занести».
    document.addEventListener('keydown', e => {
        if (e.key === 'Enter') add();
    });

    // Первичная отрисовка пустой таблицы.
    refreshState([]);
</script>
</body>
</html>
""";
}