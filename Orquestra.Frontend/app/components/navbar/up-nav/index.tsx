import SYSTEM from '@/app/consts/system';
import { handleGetGreetingDayInfo } from '@/app/functions/get.greeting';
import useWindowSize from '@/app/hooks/useWindowSize';
import styles from './index.module.scss';

export default function UpNav() {

    const windowSize = useWindowSize();

    return (
        <nav className={styles.nav}>
            {
                windowSize.width <= 801 ? (
                    <span></span>
                ) : (
                    <span>Bem-vindo ao {SYSTEM.NAME}! Tenha {handleGetGreetingDayInfo({ mustIncludeUmUma: true }).toLocaleLowerCase()} ✨</span>
                )
            }
        </nav>
    )
}