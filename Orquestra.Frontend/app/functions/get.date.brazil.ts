export default function handleGetDateBrazil(): Date {
    const o = { timeZone: 'America/Sao_Paulo' };
    const str = new Date().toLocaleString('en-US', o);
    const dt = new Date(str);

    return dt;
}