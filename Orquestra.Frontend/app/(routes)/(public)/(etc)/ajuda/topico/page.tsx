'use client';
import AjudaListRows from '@/app/(routes)/(public)/(etc)/ajuda/components/list-rows';
import AjudaSearchInput from '@/app/(routes)/(public)/(etc)/ajuda/components/seach-input';
import { HELP_TOPICS, iAjudaTopico, iAjudaTopicoItem } from '@/app/(routes)/(public)/(etc)/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import handleNormalizeUrl from '@/app/functions/format.url';
import useTitle from '@/app/hooks/useTitle';
import { useRouter, useSearchParams } from 'next/navigation';
import { Fragment, Suspense, useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function Page() {

    useTitle('Central de ajuda');

    return (
        <Suspense fallback={<div>Carregando...</div>}>
            <AjudaTopico />
        </Suspense>
    )
}

export function AjudaTopico() {

    const router = useRouter();
    const searchParams = useSearchParams();

    const [queryTopico, setQueryTopico] = useState<string>('');
    const [queryTopicoNormalized, setQueryTopicoNormalized] = useState<string>('');

    const [queryItem, setQueryItem] = useState<string>('');

    const [filteredTopicItems, setFilteredTopicItems] = useState<iAjudaTopicoItem[] | undefined>();

    useEffect(() => {
        const topico = searchParams.get('t') ?? '';
        const item = searchParams.get('i') ?? '';

        if (topico) {
            setQueryTopico(topico);
            setQueryItem('');
            return;
        }

        if (item) {
            setQueryItem(item);
            setQueryTopico('');
            return;
        }
    }, [searchParams]);

    useEffect(() => {
        if (!queryTopico) {
            return;
        }

        const queryLower = queryTopico?.toString().toLowerCase();
        const topico = HELP_TOPICS.find(x => handleNormalizeUrl(x.topic) === queryLower) as iAjudaTopico;

        setFilteredTopicItems(topico.items);
        setQueryTopicoNormalized(topico.topic);
    }, [queryTopico]);

    useEffect(() => {
        if (!queryItem) {
            return;
        }

        const queryLower = queryItem?.toString().toLowerCase();

        const filteredItems = HELP_TOPICS.flatMap(topic => {
            const topicMatch = topic.topic.toLowerCase().includes(queryLower);
            const itemsMatch = topic.items.filter(item => item.title.toLowerCase().includes(queryLower));

            if (topicMatch) {
                return topic.items.map(item => ({ ...item, topic: topic.topic }));
            }

            if (itemsMatch.length > 0) {
                return itemsMatch.map(item => ({ ...item, topic: topic.topic }));
            }

            return [];
        });

        setFilteredTopicItems(filteredItems);
    }, [queryItem]);

    return (
        <section className={styles.main}>
            <div className={`${styles.back} contrastOnHover`} onClick={() => router.push(ROUTES.ETC_AJUDA)}>
                <Seta />
                <span className='pointer'>Voltar à central de ajuda</span>
            </div>

            <div className={styles.title} >
                <span dangerouslySetInnerHTML={{ __html: queryTopicoNormalized }} />
            </div>

            {
                queryItem && (
                    <Fragment>
                        <AjudaSearchInput keySearch={queryItem?.toString() ?? ''} />

                        <div className={styles.result}>
                            <span>Resultado da busca</span>
                            <span>{filteredTopicItems?.length ?? 0} {filteredTopicItems?.length === 1 ? 'resultado' : 'resultados'}</span>
                        </div>
                    </Fragment>
                )
            }

            <AjudaListRows filteredTopicItems={filteredTopicItems?.sort((a, b) => a.title.localeCompare(b.title))} />
        </section>
    )
}