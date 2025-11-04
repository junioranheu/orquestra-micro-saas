'use client';
import ContentLoaderText from '@/app/components/content-loader/text';
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
import { Dispatch, isValidElement, JSX, MouseEvent, ReactElement, SetStateAction, useCallback, useEffect, useMemo, useState } from 'react';
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

export interface iSelectionAction {
    label: string;
    function: (ids: string[]) => void;
    icon?: StaticImageData | ReactElement;
    isButton?: boolean;
}

interface iProps {
    idPropName: string;
    columns: iTableColumn[];
    data: any[] | undefined;
    currentPage: number;
    setCurrentPage?: Dispatch<SetStateAction<number>>;
    maxPageSize?: number;
    totalRowsCount?: number;
    handleTableRowClick?: (e: any) => void;
    isMainDivBoxShadowed?: boolean;
    mainDivMarginTopBottom?: number;
    mainDivMarginSides?: number;
    mainDivHasPadding?: boolean;
    hasAltStyle?: boolean;

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

    enableRowSelection?: boolean;
    selectionAction?: iSelectionAction | null;
}

export default function TableGeneric({
    idPropName,
    columns,
    data,
    currentPage,
    setCurrentPage,
    maxPageSize = 15,
    totalRowsCount = 0,
    handleTableRowClick,
    isMainDivBoxShadowed = false,
    mainDivMarginTopBottom = 1,
    mainDivMarginSides = 3,
    mainDivHasPadding = true,
    hasAltStyle = false,

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
    setApiUrlRequest,

    enableRowSelection = false,
    selectionAction = null
}: iProps) {

    const [pageSize, setPageSize] = useState<number>(maxPageSize);
    const [countTotalItems, setCountTotalItems] = useState<number>(0);
    const [paginatedData, setPaginatedData] = useState<any[] | undefined>([]);
    const [sortColumn, setSortColumn] = useState<string | null>(null);
    const [sortOrder, setSortOrder] = useState<'ascend' | 'descend' | null>(null);
    const [showEmptyText, setShowEmptyText] = useState<boolean>(false);
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
    const [animateClass, setAnimateClass] = useState<string>('');

    // Helpers;
    const handleGetValue = useCallback((record: any, key: string): any => {
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
    }, []);

    const handleGetIdFromRecord = useCallback((record: any) => {
        const val = handleGetValue(record, idPropName);
        return val == null ? '' : String(val);
    }, [handleGetValue, idPropName]);

    const handleSort = useCallback((column: string) => {
        let newSortOrder: 'ascend' | 'descend' = 'ascend';

        if (sortColumn === column) {
            newSortOrder = sortOrder === 'ascend' ? 'descend' : 'ascend';
        }

        const sorted = [...(paginatedData ?? [])].sort((a, b) => {
            if (a[column] < b[column]) {
                return newSortOrder === 'ascend' ? -1 : 1;
            }

            if (a[column] > b[column]) {
                return newSortOrder === 'ascend' ? 1 : -1;
            }

            return 0;
        });

        setPaginatedData(sorted);
        setSortColumn(column);
        setSortOrder(newSortOrder);
    }, [paginatedData, sortColumn, sortOrder]);

    const handleMakeSelectionColumn = useCallback(() => ({
        title: (
            <div style={{ textAlign: 'center', width: '100%' }}>
                <input
                    type='checkbox'
                    aria-label='Selecionar todos'
                    checked={paginatedData ? paginatedData.every(r => selectedIds.has(handleGetIdFromRecord(r))) && paginatedData.length > 0 : false}
                    onChange={(e) => {
                        const checked = e.target.checked;
                        const newSet = new Set(selectedIds);

                        (paginatedData ?? []).forEach(r => {
                            const id = handleGetIdFromRecord(r);

                            if (checked) {
                                newSet.add(id)
                            } else {
                                newSet.delete(id);
                            }
                        });

                        setSelectedIds(newSet);
                    }}
                />
            </div>
        ),
        key: '__select__',
        width: 40,
        align: 'center' as const,
        render: (_: any, record: any) => {
            const id = handleGetIdFromRecord(record);
            const checked = selectedIds.has(id);
            return (
                <div style={{ textAlign: 'center' }}>
                    <input
                        type='checkbox'
                        aria-label={`Selecionar linha ${id}`}
                        checked={checked}
                        onChange={(e) => {
                            const newSet = new Set(selectedIds);

                            if (e.target.checked) {
                                newSet.add(id)
                            } else {
                                newSet.delete(id);
                            }

                            setSelectedIds(newSet);
                        }}
                        onClick={(ev) => ev.stopPropagation()}
                    />
                </div>
            );
        }
    }), [paginatedData, selectedIds, handleGetIdFromRecord]);

    // Effects;
    useEffect(() => {
        const total = Number(totalRowsCount) || 0;
        setCountTotalItems(total);
        const stablePageSize = maxPageSize > 0 ? maxPageSize : 15;
        setPageSize(stablePageSize);

        if (!data || !data.length) {
            return setPaginatedData([]);
        }

        if (total > 0) {
            return setPaginatedData(data);
        }

        setPaginatedData(data.slice((currentPage - 1) * stablePageSize, currentPage * stablePageSize));
    }, [data, currentPage, totalRowsCount, maxPageSize]);

    useEffect(() => setSelectedIds(new Set()), [data, currentPage, pageSize]);

    useEffect(() => {
        setShowEmptyText(false);
        const timer = setTimeout(() => setShowEmptyText(true), 1000);
        return () => clearTimeout(timer);
    }, [currentPage]);

    // Memoized columns;
    const enhancedColumns = useMemo(() => columns.map(col => ({
        ...col,
        title: (
            <span onClick={() => handleSort(col.dataIndex)}>
                {col.title} {sortColumn === col.dataIndex && (sortOrder === 'ascend' ? '↑' : '↓')}
            </span>
        ),
        onHeaderCell: () => ({
            onClick: () => handleSort(col.dataIndex),
            style: { cursor: 'pointer', textDecoration: sortColumn === col.dataIndex ? 'underline' : 'none' }
        }),
        render: (value: any, record: any, index: number) => {
            const val = handleGetValue(record, col.dataIndex);
            return col.render ? col.render(val, record, index) : val;
        }
    })), [columns, sortColumn, sortOrder, handleSort, handleGetValue]);

    const finalColumns = useMemo(() => {
        const cols = [...enhancedColumns];

        if (enableRowSelection) {
            cols.unshift(handleMakeSelectionColumn() as any);
        }

        if (managingOptions?.length > 0) {
            cols.push({
                title: (<span style={{ display: 'block', textAlign: 'center', width: '100%' }}>Ações</span>),
                key: 'actions',
                width: 100,
                render: (record: any) => (
                    <div className={styles.column_actions}>
                        {
                            managingOptions.map((option, index) => (option.isButton ? (
                                <Button
                                    key={index}
                                    label={option.label}
                                    handleFunction={() => option.function(record)}
                                    svg_staticImageData={!isValidElement(option.icon) ? (option.icon as StaticImageData) : null}
                                    icon_feather={isValidElement(option.icon) ? (option.icon as JSX.Element) : null}
                                    isStyleSimple={false}
                                />
                            ) : (
                                <Tippy key={index} content={option.label}>
                                    <span onClick={() => option.function(record)}>
                                        {isValidElement(option.icon) ? option.icon : <Image src={option.icon} alt={option.label} />}
                                    </span>
                                </Tippy>
                            )))
                        }
                    </div>
                )
            } as any);
        }
        return cols;
    }, [enhancedColumns, enableRowSelection, managingOptions, handleMakeSelectionColumn]);

    // Pagination;
    const handlePageChange = useCallback((page: number) => {
        setCurrentPage?.(page);
        setAnimateClass('');
        setTimeout(() => setAnimateClass(SYSTEM.ANIMATE_SLOW), 10);
    }, [setCurrentPage]);

    const handleSelectionAction = useCallback(() => {
        if (!selectionAction?.function) {
            return;
        }

        selectionAction.function(Array.from(selectedIds));
    }, [selectionAction, selectedIds]);

    const totalPages = pageSize > 0 ? Math.ceil(countTotalItems / pageSize) : 1;
    const safeTotal = totalPages * pageSize;

    return (
        <section
            className={`${styles.main} ${(hasAltStyle && 'tableAltStyle')} ${(hasAltStyle && styles.tableAltStyle)}`}
            style={{
                boxShadow: isMainDivBoxShadowed ? 'var(--box-shadow)' : '',
                margin: `${mainDivMarginTopBottom}rem ${mainDivMarginSides}rem`,
                padding: mainDivHasPadding ? '1rem' : '0rem'
            }}
        >
            <div className={styles.container}>
                {
                    (title || extraItems?.length || btn_filter_label || btn_import_label || btn_export_label || btn_add_label || (enableRowSelection && selectionAction)) && (
                        <div className={styles.top}>
                            {
                                title && (
                                    <div className={styles.left}>
                                        <Tippy content={`${totalRowsCount} registro${totalRowsCount > 1 ? 's' : ''}.`}>
                                            <span className={styles.title}>
                                                <ContentLoaderText content={title} delay={150} />
                                            </span>
                                        </Tippy>
                                    </div>
                                )
                            }

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
                                    btn_filter_label && btn_filter_function && (
                                        <Button
                                            label={btn_filter_label}
                                            handleFunction={btn_filter_function}
                                            isStyleSimple
                                            icon_feather={<Icon icon='search' size='small' />}
                                            style={{ fontSize: '0.75rem', backgroundColor: 'var(--white-og)' }}
                                        />
                                    )
                                }

                                {
                                    btn_import_label && btn_import_function && (
                                        <Button
                                            label={btn_import_label}
                                            handleFunction={btn_import_function}
                                            isStyleSimple
                                            icon_feather={<Icon icon='upload' size='small' />}
                                            style={{ fontSize: '0.75rem', backgroundColor: 'var(--white-og)' }}
                                        />
                                    )
                                }

                                {
                                    btn_export_label && btn_export_function && (
                                        <Button
                                            label={btn_export_label}
                                            handleFunction={btn_export_function}
                                            isStyleSimple
                                            icon_feather={<Icon icon='download' size='small' />}
                                            style={{ fontSize: '0.75rem', backgroundColor: 'var(--white-og)' }}
                                        />
                                    )
                                }

                                {
                                    btn_add_label && btn_add_function && (
                                        <Button
                                            label={btn_add_label}
                                            handleFunction={btn_add_function}
                                            icon_feather={<Icon icon='plus-circle' size='small' />}
                                            style={{ fontSize: '0.75rem' }}
                                        />
                                    )
                                }

                                {
                                    enableRowSelection && selectionAction && (
                                        <Button
                                            label={`${selectionAction.label}${selectedIds.size > 0 ? ` (${selectedIds.size})` : ''}`}
                                            handleFunction={handleSelectionAction}
                                            isStyleSimple={false}
                                            svg_staticImageData={!isValidElement(selectionAction.icon) ? (selectionAction.icon as StaticImageData) : null}
                                            icon_feather={isValidElement(selectionAction.icon) ? (selectionAction.icon as JSX.Element) : <Icon icon='check' size='small' />}
                                            style={{ fontSize: '0.75rem', marginLeft: '.5rem' }}
                                            isDisabled={selectedIds.size === 0}
                                        />
                                    )
                                }
                            </div>
                        </div>
                    )
                }

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
                columns={finalColumns}
                data={paginatedData}
                rowKey={idPropName}
                onRow={(row) => ({ onClick: handleTableRowClick ? () => handleTableRowClick(row) : undefined })}
                emptyText={showEmptyText ? <span className={animateClass}>Os dados ainda estão carregando ou não existem dados para exibir neste momento</span> : null}
                className={SYSTEM.ANIMATE}
                rowClassName={animateClass}
            />

            <Pagination
                current={currentPage}
                pageSize={pageSize}
                total={safeTotal}
                showSizeChanger
                onChange={handlePageChange}
                onShowSizeChange={(size: number) => {
                    setPageSize(size);
                    setCurrentPage?.(1);
                }}
            />
        </section>
    )
}