import styles from './index.module.scss';

interface iParams<T> {
    titles: string[];
    data: T[] | undefined;
}

export default function TableSimple<T>({ titles, data }: iParams<T>) {

    if (!titles || !data || data?.length < 1) {
        return;
    }

    return (
        <table className={styles.table}>
            <thead>
                <tr>
                    {
                        titles?.map((title: string, index: number) => (
                            <th key={index}>{title}</th>
                        ))
                    }
                </tr>
            </thead>

            <tbody>
                {
                    data?.map((item: T, index: number) => (
                        <tr key={index}>
                            {
                                Object.keys(item as any).map((key) => (
                                    <td key={key}>{(item as any)[key]}</td>
                                ))
                            }
                        </tr>
                    ))
                }
            </tbody>
        </table>
    )
}