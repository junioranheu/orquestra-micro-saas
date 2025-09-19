import { iAjudaTopico } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import ROUTES from '@/app/consts/routes';
import handleNormalizeUrl, { handleNormalizeHtml } from '@/app/functions/format.url';
import { useRouter } from 'next/navigation';
import styles from './index.module.scss';

interface iProps {
    topics: iAjudaTopico[];
}

export default function AjudaListCards({ topics }: iProps) {

    const router = useRouter();

    return (
        <div className={styles.main}>
            {
                topics?.map((item: iAjudaTopico, i: number) => (
                    <div
                        key={i}
                        className={styles.topic}
                        onClick={() => router.push(`${ROUTES.ETC_AJUDA}/topico/${handleNormalizeUrl(handleNormalizeHtml(item?.topic))}`)}
                    >
                        <div className={styles.title} dangerouslySetInnerHTML={{ __html: item?.topic }} />
                        <span className={styles.subtitle}>{item?.description}</span>
                        <span className={styles.more}>Saiba mais</span>
                    </div>
                ))
            }
        </div>
    )
}