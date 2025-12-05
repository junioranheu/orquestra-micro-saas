import { iLogNotificationOutput } from '@/app/api/consts/log';
import styles from './index.module.scss';

interface iProps {
    notification: iLogNotificationOutput;
}

export default function NotificationJsonVisualize({ notification }: iProps) {

    const changed = typeof notification.changedFields === 'object'
        ? JSON.stringify(notification.changedFields, null, 2)
        : notification.changedFields;

    return (
        <pre className={styles.codeBlock}>
            <code
                className={styles.json}
                dangerouslySetInnerHTML={{ __html: changed ?? '' }}
            />
        </pre>
    )
}