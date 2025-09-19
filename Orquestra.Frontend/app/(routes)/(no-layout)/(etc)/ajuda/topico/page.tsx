'use client';
import AjudaListRows from '@/app/(routes)/(no-layout)/(etc)/ajuda/components/list-rows';
import AjudaSearchInput from '@/app/(routes)/(no-layout)/(etc)/ajuda/components/seach-input';
import { HELP_TOPICS, iAjudaTopico, iAjudaTopicoItem } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import Seta from '@/app/components/svg/seta/seta';
import ROUTES from '@/app/consts/routes';
import handleNormalizeUrl from '@/app/functions/format.url';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import styles from './page.module.scss';

export default function AjudaTopico() {

    useTitle('Central de ajuda');
    const router = useRouter();

    const [queryTopico, setqueryTopico] = useState<string>('');
    const [queryTopicoNormalized, setQueryTopicoNormalized] = useState<string>('');

    const [queryItem, setQueryItem] = useState<string>('');

    const [filteredTopicItems, setFilteredTopicItems] = useState<iAjudaTopicoItem[] | undefined>();

    useEffect(() => {
        // Função para atualizar os valores com base na URL;
        function handleUpdateQuery() {
            const url = window.location.search;
            const topico = new URLSearchParams(url).get('t') ?? '';

            if (topico) {
                setqueryTopico(topico);
                return;
            }

            const item = new URLSearchParams(url).get('i') ?? '';
            setQueryItem(item);
        };

        handleUpdateQuery(); // Executa na montagem;

        // Workaround: intercepta router.push para atualizar a URL;
        const originalPush = router.push;

        router.push = (url: string, ...args: any) => {
            const result = originalPush(url, ...args);
            handleUpdateQuery();

            return result;
        };

        return () => {
            router.push = originalPush;
        }
    }, [router]);

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
            // setFilteredTopicItems(HELP_TOPICS.flatMap(x => x.items));
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
                    <div>
                        <AjudaSearchInput keySearch={queryItem?.toString() ?? ''} />

                        <div className={styles.result}>
                            <span>Resultado da busca</span>
                            <span>{filteredTopicItems?.length ?? 0} {filteredTopicItems?.length === 1 ? 'resultado' : 'resultados'}</span>
                        </div>
                    </div>
                )
            }
            <AjudaListRows filteredTopicItems={filteredTopicItems} />
        </section>
    )
}