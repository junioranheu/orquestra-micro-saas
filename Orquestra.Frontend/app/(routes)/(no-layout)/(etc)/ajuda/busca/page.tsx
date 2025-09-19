'use client';
import AjudaListRows from '@/app/(routes)/(no-layout)/(etc)/ajuda/item/list-rows';
import { HELP_TOPICS, iAjudaTopicoItem } from '@/app/(routes)/(no-layout)/(etc)/ajuda/page';
import styles from '@/app/(routes)/(no-layout)/(etc)/ajuda/page.module.scss';
import AjudaSearchInput from '@/app/(routes)/(no-layout)/(etc)/ajuda/seach-input';
import ImgMeditation from '@/app/assets/webp/meditation.webp';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import { useSearchParams } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function AjudaBusca() {

    useTitle('Central de ajuda');
    const searchParams = useSearchParams();
    const query = searchParams.get('q');

    const [filteredTopicItems, setFilteredTopicItems] = useState<iAjudaTopicoItem[]>();

    useEffect(() => {
        const queryLower = query?.toString().toLowerCase();

        if (!queryLower) {
            setFilteredTopicItems(HELP_TOPICS.flatMap(x => x.items));
            return;
        }

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
    }, [query]);

    return (
        <section className={styles.main}>
            <div className={styles.hero}>
                <span>Central de ajuda</span>

                <div className={SYSTEM.ANIMATE_PULSE_INFINITE}>
                    <Image src={ImgMeditation} alt='' priority={true} />
                </div>
            </div>

            <AjudaSearchInput keySearch={query?.toString() ?? ''} />

            <div className={styles.result}>
                <span>Resultado da busca</span>
                <span>{filteredTopicItems?.length ?? 0} {filteredTopicItems?.length === 1 ? 'resultado' : 'resultados'}</span>
            </div>

            <AjudaListRows filteredTopicItems={filteredTopicItems} />
        </section>
    )
}