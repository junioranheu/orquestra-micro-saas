'use client';
import { HELP_TOPICS, iAjudaTopicoItem } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import handleNormalizeUrl, { handleNormalizeHtml } from '@/app/functions/format.url';
import useTitle from '@/app/hooks/useTitle';
import Tippy from '@tippyjs/react';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function AjudaItem() {

    useTitle('Central de ajuda');
    const router = useRouter();
    const params = useParams();
    const query = params.topico;

    const [filteredTopicItem, setFilteredTopicItem] = useState<iAjudaTopicoItem | null>();
    const [originalTopic, setOriginalTopic] = useState<string>('');

    function handleFindItem(title: string): iAjudaTopicoItem | null {
        for (const topic of HELP_TOPICS) {
            const item = topic.items.find(x => handleNormalizeUrl(handleNormalizeHtml(x?.title)) === title) as iAjudaTopicoItem;

            if (item) {
                setOriginalTopic(topic.topic);
                return item;
            }
        }

        return null;
    }

    useEffect(() => {
        if (query) {
            const result = handleFindItem(query.toString());

            if (!result) {
                router.push(ROUTES.ERRO_404);
                return;
            }

            setFilteredTopicItem(result);
        }
    }, [query, router]);

    if (!filteredTopicItem) {
        return;
    }

    return (
        <section className={styles.main}>
            <div
                className={`${styles.back} contrastOnHover`}
                onClick={() => router.push(`${ROUTES.ETC_AJUDA}/topico?t=${handleNormalizeUrl(handleNormalizeHtml(originalTopic))}`)}
            >
                <Seta />

                <Tippy content={originalTopic}>
                    <span className='pointer'>Voltar ao tópico original</span>
                </Tippy>
            </div>

            <div className={styles.title}>
                <span className={styles.small}>Central de ajuda / {originalTopic} /</span>
                <span className={styles.regular}>{filteredTopicItem?.title}</span>
            </div>

            <div className={styles.content}>
                <span dangerouslySetInnerHTML={{ __html: (filteredTopicItem?.description ?? '') }} />
            </div>
        </section>
    )
}