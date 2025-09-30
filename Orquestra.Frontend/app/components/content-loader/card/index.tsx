import { Fragment } from 'react';
import ContentLoader from 'react-content-loader';
import styles from './index.module.scss';

type iProps = {
    showHeader?: boolean
    id?: string;
}

export function ContentLoaderCard({ showHeader = false, id = "default" }: iProps) {
    return (
        <ContentLoader
            uniqueKey={`content-loader-card-${id}`}
            viewBox='0 0 400 300'
            width='100%'
            height='100%'
            preserveAspectRatio='none'
            backgroundColor='#eee'
            foregroundColor='#ddd'
            speed={1.75}
            title=''
            style={{ width: '100%', height: '100%', maxHeight: '14rem' }}
        >
            {
                showHeader && (
                    <Fragment>
                        <rect x='0' y='13' rx='4' ry='4' width='100%' height='9' />
                        <rect x='0' y='29' rx='4' ry='4' width='25%' height='8' />
                        <rect x='0' y='50' rx='4' ry='4' width='100%' height='10' />
                        <rect x='0' y='65' rx='4' ry='4' width='100%' height='10' />
                        <rect x='0' y='79' rx='4' ry='4' width='25%' height='10' />
                    </Fragment>
                )
            }

            <rect x='0' y={showHeader ? '99' : '0'} rx='8' ry='8' width='100%' height={showHeader ? '200' : '100%'} />
        </ContentLoader>
    )
}

export function ContentLoaderCardGrid() {
    return (
        <div className={styles.grid} suppressHydrationWarning={true}>
            <ContentLoaderCard />
            <ContentLoaderCard />
        </div>
    )
}