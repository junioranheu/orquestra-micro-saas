export function handleGetDateBrazil(): Date {
    const o = { timeZone: 'America/Sao_Paulo' };
    const str = new Date().toLocaleString('en-US', o);
    const dt = new Date(str);

    return dt;
}

export function handleToBrazilDate(date: Date | string): Date {
    const d = typeof date === 'string' ? new Date(date) : date;

    // formata essa data no fuso de Brasília;
    const parts = new Intl.DateTimeFormat('en-US', {
        timeZone: 'America/Sao_Paulo',
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false,
    }).formatToParts(d);

    const get = (type: string) => Number(parts.find(p => p.type === type)?.value);

    // monta um novo Date, interpretando como horário de Brasília;
    return new Date(
        get('year'),
        get('month') - 1,
        get('day'),
        get('hour'),
        get('minute'),
        get('second')
    );
}