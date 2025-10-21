'use client';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import FiltersSelected from '@/app/components/table/filters-selected';
import SYSTEM from '@/app/consts/system';
import Tippy from '@tippyjs/react';
import Image, { StaticImageData } from 'next/image';
import Pagination from 'rc-pagination';
import 'rc-pagination/assets/index.css';
import Table, { ColumnType as RcTableColumnType } from 'rc-table';
import 'rc-table/assets/index.css';
import { Dispatch, isValidElement, JSX, MouseEvent, ReactElement, SetStateAction, useEffect, useState } from 'react';
import styles from './index.module.scss';

export interface iTableColumn extends RcTableColumnType<any> {
    dataIndex: string;
    key: string;
}

export interface iTableExtraItems {
    title?: string;
    label: string;
}

export interface iTableManagingOptions {
    label: string;
    function: (record: any) => void;
    icon: StaticImageData | ReactElement;
    isButton?: boolean;
}

interface iProps {
    idPropName: string;
    columns: iTableColumn[];
    data: any[] | undefined;
    currentPage: number; // É necessário usar o currentPage como param para posteriormente utilizá-lo para requisições ao back-end (por isso o currentPage não está internamente no componente);
    setCurrentPage?: Dispatch<SetStateAction<number>>;
    maxPageSize?: number;
    totalRowsCount?: number;
    handleTableRowClick?: (e: any) => void;
    isMainDivBoxShadowed?: boolean;
    mainDivMarginTopBottom?: number;
    mainDivMarginSides?: number;
    mainDivHasPadding?: boolean;

    title?: string;
    managingOptions?: iTableManagingOptions[];
    extraItems?: iTableExtraItems[] | null;
    btn_add_label?: string;
    btn_add_function?: (e: MouseEvent<HTMLDivElement>) => void;
    btn_import_label?: string;
    btn_import_function?: (e: MouseEvent<HTMLDivElement>) => void;
    btn_export_label?: string;
    btn_export_function?: (e: MouseEvent<HTMLDivElement>) => void;
    btn_filter_label?: string;
    btn_filter_function?: (e: MouseEvent<HTMLDivElement>) => void;

    modalFilterFormData?: any;
    setModalFilterFormData?: Dispatch<SetStateAction<any>>;
    apiUrlRequest?: string;
    setApiUrlRequest?: Dispatch<SetStateAction<string>>;
}

export default function TableGeneric({
    idPropName,
    columns,
    data,
    currentPage,
    setCurrentPage,
    maxPageSize = 25,
    totalRowsCount = 0,
    handleTableRowClick = undefined,
    isMainDivBoxShadowed = false,
    mainDivMarginTopBottom = 1,
    mainDivMarginSides = 3,
    mainDivHasPadding = true,

    title,
    managingOptions = [],
    extraItems = null,
    btn_add_label,
    btn_add_function,
    btn_import_label,
    btn_import_function,
    btn_export_label,
    btn_export_function,
    btn_filter_label,
    btn_filter_function,
    modalFilterFormData,
    setModalFilterFormData,
    apiUrlRequest,
    setApiUrlRequest
}: iProps) {

    const [pageSize, setPageSize] = useState<number>(0);
    const [countTotalItems, setCountTotalItems] = useState<number>(0);
    const [paginatedData, setPaginatedData] = useState<any[] | undefined>([]);
    const [sortColumn, setSortColumn] = useState<string | null>(null);
    const [sortOrder, setSortOrder] = useState<'ascend' | 'descend' | null>(null);
    const [showEmptyText, setShowEmptyText] = useState(false);

    useEffect(() => {
        function handlePagination() {
            if (totalRowsCount > 0) {
                // console.log(`Foi definido que há um limite de linhas (totalRowsCount: ${totalRowsCount}})`);
                setCountTotalItems(totalRowsCount ?? 1);

                // Workaround;
                setPaginatedData([]);

                setTimeout(() => {
                    // console.log(data?.length, data);
                    setPaginatedData(data);
                }, 10);

                if (pageSize < 1) {
                    const pageSizeHandlePagination = data?.length ?? maxPageSize;
                    // console.log(`Definir o setPageSize apenas uma vez: ${pageSizeHandlePagination}`);

                    setPageSize(pageSizeHandlePagination);
                }

                return;
            }

            // console.log('NÃO foi definido que há um limite de linhas (totalRowsCount)');
            setCountTotalItems(data?.length ?? 1);
            setPageSize(maxPageSize);

            const paginatedData = data?.slice((currentPage - 1) * maxPageSize, currentPage * maxPageSize);

            // Workaround;
            setPaginatedData([]);

            setTimeout(() => {
                setPaginatedData(paginatedData);
            }, 10);
        }

        if (data?.length) {
            handlePagination();
        } else {
            setTimeout(() => {
                setPaginatedData([]);
            }, 10);
        }
    }, [data, currentPage, totalRowsCount, maxPageSize, pageSize]);

    const [animateClass, setAnimateClass] = useState<string>('');

    function handlePageChange(page: number) {
        if (setCurrentPage) {
            setCurrentPage(page);
        }

        setAnimateClass('');

        setTimeout(() => {
            setAnimateClass(SYSTEM.ANIMATE_SLOW);
        }, 10);
    }

    function handleSort(column: string) {
        let newSortOrder: 'ascend' | 'descend' = 'ascend';

        if (sortColumn === column) {
            newSortOrder = sortOrder === 'ascend' ? 'descend' : 'ascend';
        }

        const sorted = [...paginatedData ?? []].sort((a, b) => {
            if (a[column] < b[column]) return newSortOrder === 'ascend' ? -1 : 1;
            if (a[column] > b[column]) return newSortOrder === 'ascend' ? 1 : -1;
            return 0;
        });

        setPaginatedData(sorted);
        setSortColumn(column);
        setSortOrder(newSortOrder);
    }

    function handleGetValue(record: any, key: string): any {
        if (!record || !key) {
            return undefined;
        }

        const parts = key.split('.');
        let current = record;

        for (const part of parts) {
            if (current == null) {
                return undefined;
            }

            const match = Object.keys(current).find(k => k.toLowerCase() === part.toLowerCase());

            current = match ? current[match] : undefined;
        }

        return current;
    }

    const enhancedColumns = columns.map(col => ({
        ...col,
        title: (
            <span onClick={() => handleSort(col.dataIndex)}>
                {col.title} {sortColumn === col.dataIndex && (sortOrder === 'ascend' ? '↑' : '↓')}
            </span>
        ),
        onHeaderCell: () => ({
            onClick: () => handleSort(col.dataIndex),
            style: {
                cursor: 'pointer',
                textDecoration: sortColumn === col.dataIndex ? 'underline' : 'none'
            }
        }),
        render: (value: any, record: any, index: number) => {
            const val = handleGetValue(record, col.dataIndex);
            return col.render ? col.render(val, record, index) : val;
        }
    }))

    // Add managing options column if managingOptions is provided;
    if (managingOptions?.length > 0) {
        // @ts-expect-error: a definição de tipo de iTableColumn não permite esse render customizado, mas é intencional aqui;
        enhancedColumns.push({
            title: (<span>Ações</span>),
            key: 'actions',
            width: 100,
            render: (record: any) => (
                <div className={styles.column_actions}>
                    {
                        managingOptions?.map((option: iTableManagingOptions, index: number) => {
                            // Verifica se é um botão ou um ícone normal;
                            if (option.isButton) {
                                return (
                                    <Button
                                        key={index}
                                        label={option.label}
                                        handleFunction={() => option.function(record)}
                                        svg_staticImageData={!isValidElement(option.icon) ? (option.icon as StaticImageData) : null}
                                        icone_feather={isValidElement(option.icon) ? (option.icon as JSX.Element) : null}
                                        isStyleSimple={false}
                                    />
                                )
                            } else {
                                return (
                                    <Tippy key={index} content={option.label}>
                                        <span onClick={() => option.function(record)}>
                                            {
                                                // Verifica se o ícone é um componente React ou uma imagem estática;
                                                isValidElement(option.icon) ? (
                                                    option.icon // Se for um componente React, renderiza ele;
                                                ) : (
                                                    <Image src={option.icon} alt={option.label} /> // Caso contrário, trata como uma imagem;
                                                )
                                            }
                                        </span>
                                    </Tippy>
                                )
                            }
                        })
                    }
                </div>
            )
        });
    }

    useEffect(() => {
        setShowEmptyText(false);

        const timer = setTimeout(() => {
            setShowEmptyText(true);
        }, 1000);

        return () => clearTimeout(timer);
    }, [currentPage]);

    return (
        <section
            className={styles.main}
            style={{
                boxShadow: (isMainDivBoxShadowed ? 'var(--box-shadow)' : ''),
                margin: `${mainDivMarginTopBottom}rem ${mainDivMarginSides}rem`,
                padding: (mainDivHasPadding ? '1rem' : '0rem')
            }}
        >
            <div className={styles.container}>
                <div className={styles.top}>
                    <div className={styles.left}>
                        <span className={styles.title} dangerouslySetInnerHTML={{ __html: title ?? '' }} />
                    </div>

                    <div className={styles.right}>
                        {
                            extraItems?.map((x, i) => (
                                <div key={i} className={styles.extraItem}>
                                    {x.title && <span className={styles.title}>{x.title}</span>}
                                    <span className={styles.label}>{x.label}</span>
                                </div>
                            ))
                        }

                        {
                            (btn_filter_label && btn_filter_function) && (
                                <Button
                                    label={btn_filter_label}
                                    handleFunction={btn_filter_function}
                                    isStyleSimple={true}
                                    icone_feather={<Icon icon='search' size='small' />}
                                    style={{ fontSize: '0.75rem' }}
                                />
                            )
                        }

                        {
                            (btn_import_label && btn_import_function) && (
                                <Button
                                    label={btn_import_label}
                                    handleFunction={btn_import_function}
                                    isStyleSimple={true}
                                    icone_feather={<Icon icon='upload' size='small' />}
                                    style={{ fontSize: '0.75rem' }}
                                />
                            )
                        }

                        {
                            (btn_export_label && btn_export_function) && (
                                <Button
                                    label={btn_export_label}
                                    handleFunction={btn_export_function}
                                    isStyleSimple={true}
                                    icone_feather={<Icon icon='download' size='small' />}
                                    style={{ fontSize: '0.75rem' }}
                                />
                            )
                        }

                        {
                            (btn_add_label && btn_add_function) && (
                                <Button
                                    label={btn_add_label}
                                    handleFunction={btn_add_function}
                                    icone_feather={<Icon icon='plus-circle' size='small' />}
                                    style={{ fontSize: '0.75rem' }}
                                />
                            )
                        }
                    </div>
                </div>

                {
                    (apiUrlRequest && setApiUrlRequest) && (
                        <div className={styles.bottom}>
                            <FiltersSelected
                                modalFilterFormData={modalFilterFormData}
                                setModalFilterFormData={setModalFilterFormData}
                                apiUrlRequest={apiUrlRequest}
                                setApiUrlRequest={setApiUrlRequest}
                            />
                        </div>
                    )
                }
            </div>

            <Table
                columns={enhancedColumns}
                data={paginatedData}
                rowKey={idPropName}
                onRow={(row) => ({
                    onClick: handleTableRowClick ? () => handleTableRowClick(row) : () => null,
                })}
                emptyText={(showEmptyText ? <span className={animateClass}>Os dados ainda estão carregando ou não existem dados para exibir neste momento</span> : null)}
                className={SYSTEM.ANIMATE}
                rowClassName={animateClass}
            />

            <Pagination
                current={currentPage}
                pageSize={pageSize}
                total={countTotalItems}
                showSizeChanger={true}
                onChange={handlePageChange}
                onShowSizeChange={(current: number, size: number) => setPageSize(size)}
            />
        </section>
    )
}