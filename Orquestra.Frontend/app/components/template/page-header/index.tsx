import { ContentLoaderCardGrid } from '@/app/components/content-loader/card';
import ContentLoaderGrid from '@/app/components/content-loader/grid';
import { Fragment, ReactNode } from 'react';
import styles from './index.module.scss';

interface iProps {
    title: string;
    isLoading: boolean;
    actions?: ReactNode[];
    children?: ReactNode;
}

export default function TemplatePageHeader({ title, isLoading, actions = [], children }: iProps) {
    return (
        <section className={styles.main}>
            <div className={styles.pageHeader}>
                <h1 className={styles.pageTitle}>{title}</h1>

                {
                    actions?.length > 0 && (
                        <div className={styles.actions}>
                            {
                                actions?.map((action, index) => (
                                    <Fragment key={index}>{action}</Fragment>
                                ))
                            }
                        </div>
                    )
                }
            </div>

            {
                isLoading ? (
                    <Fragment>
                        <ContentLoaderCardGrid />
                        <ContentLoaderGrid row={1} />
                    </Fragment>
                ) : (
                    children
                )
            }
        </section>
    )
}