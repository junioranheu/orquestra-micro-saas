import { HELP_TOPICS, iAjudaTopico } from '@/app/(routes)/(no-layout)/etc/ajuda/page';
import handleNormalizeUrl, { handleNormalizeHtml } from '@/app/functions/format.url';
import { useRouter } from 'next/navigation';
import styles from './ajuda.topicos.module.scss';

export default function AjudaTopics() {

    const router = useRouter();

    return (
        <div className={styles.main}>
            {
                HELP_TOPICS?.map((item: iAjudaTopico, i: number) => (
                    <div
                        key={i}
                        className={styles.topic}
                        onClick={() => router.push(`/ajuda/topico/${handleNormalizeUrl(handleNormalizeHtml(item?.topic))}`)}
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