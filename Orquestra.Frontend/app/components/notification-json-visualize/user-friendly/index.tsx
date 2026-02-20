import { iLogNotificationOutput } from '@/app/api/consts/log';
import styles from './index.module.scss';

interface iProps {
    notification: iLogNotificationOutput;
}

export default function NotificationFriendlyVisualize({ notification }: iProps) {

    const parsed = handleParseChangedFields(notification.changedFields);

    if (!parsed) {
        return <span className={styles.empty}>Sem alterações</span>;
    }

    const entries = Object.entries(parsed);

    return (
        <div className={styles.wrapper}>
            {
                entries?.map(function ([field, value]) {
                    const diff = handleExtractDiff(value);

                    return (
                        <div key={field} className={styles.row}>
                            <span className={styles.tag}>{field}</span>

                            {
                                diff ? (
                                    <span className={styles.diff}>
                                        <span className={styles.old}>{handleFormatValue(diff.old)}</span>
                                        <span className={styles.arrow}>→</span>
                                        <span className={styles.new}>{handleFormatValue(diff.new)}</span>
                                    </span>
                                ) : (
                                    <span className={styles.updated}>{handleFormatValue(value)}</span>
                                )
                            }
                        </div>
                    );
                })
            }
        </div>
    );
}

/* ============================= */
/* helpers */
/* ============================= */
function handleParseChangedFields(raw: any): Record<string, any> | null {
    if (!raw) {
        return null;
    }

    if (typeof raw === 'object') {
        return raw;
    }

    if (typeof raw === 'string') {
        try {
            return JSON.parse(raw);
        } catch {
            return null;
        }
    }

    return null;
}

function handleExtractDiff(value: any): { old?: any; new?: any } | null {
    if (!value || typeof value !== 'object') {
        return null;
    }

    // Português;
    if ('Antes' in value || 'Depois' in value) {
        return {
            old: value.Antes,
            new: value.Depois,
        };
    }

    // Padrões comuns;
    if ('old' in value || 'new' in value) {
        return { old: value.old, new: value.new };
    }

    if ('before' in value || 'after' in value) {
        return { old: value.before, new: value.after };
    }

    if ('previous' in value || 'current' in value) {
        return { old: value.previous, new: value.current };
    }

    if ('from' in value || 'to' in value) {
        return { old: value.from, new: value.to };
    }

    return null;
}

function handleFormatValue(value: any): string {
    if (value === null || value === undefined) {
        return '—';
    }

    if (typeof value === 'boolean') {
        return value ? 'Sim' : 'Não';
    }

    if (typeof value === 'number') {
        return String(value);
    }

    if (typeof value === 'string') {
        if (!value.trim()) {
            return '—';
        }

        if (/^\d{4}-\d{2}-\d{2}T/.test(value)) {
            try {
                return new Date(value).toLocaleString('pt-BR');
            } catch {
                return value;
            }
        }

        return value;
    }

    if (Array.isArray(value)) {
        return `${value.length} item(ns)`;
    }

    return 'Atualizado';
}