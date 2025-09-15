import SYSTEM from '@/app/consts/system';
import { handleGetGreetingDayInfo } from '@/app/functions/get.greeting';
import styles from './index.module.scss';

export default function UpNav() {
    return (
        <nav className={styles.nav}>
            <span>Bem-vindo ao {SYSTEM.NAME}! Tenha {handleGetGreetingDayInfo({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨</span>
        </nav>
    )
}