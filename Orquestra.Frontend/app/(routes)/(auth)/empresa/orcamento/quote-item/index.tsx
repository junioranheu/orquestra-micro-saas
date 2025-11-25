import { iQuote, iQuoteItem } from '@/app/api/consts/quote';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import { Guid } from 'guid-typescript';
import { useEffect } from 'react';
import styles from './index.module.scss';

interface iProps {
    formData: iQuote;
    setFormData: (cb: (prev: iQuote) => iQuote) => void;
    editing: boolean;
}

export default function ItemsEditor({ formData, setFormData, editing }: iProps) {

    const LABEL_BTN_ADD = 'Adicionar novo item';

    function handleRecalcTotals(items: iQuoteItem[]) {
        return items.map((it) => ({
            ...it,
            totalPrice: Number(((it.quantity ?? 0) * (it.unitPrice ?? 0)).toFixed(2))
        }));
    }

    function handleAddItem() {
        const newItem: iQuoteItem = {
            quoteItemId: Guid.create(),
            title: '',
            description: '',
            quantity: 1,
            unitPrice: 0,
            totalPrice: 0,
        };

        setFormData(prev => ({
            ...prev,
            items: handleRecalcTotals([...(prev.items ?? []), newItem])
        }));
    }

    function handleRemoveItem(id: Guid) {
        setFormData(prev => ({
            ...prev,
            items: handleRecalcTotals(prev.items!.filter(i => i.quoteItemId !== id))
        }));
    }

    function handleUpdateItem(id: Guid, field: keyof Omit<iQuoteItem, 'quoteItemId' | 'totalPrice'>, value: any, min?: number, max?: number) {
        setFormData(prev => {
            const items = prev.items!.map(it => {
                if (it.quoteItemId !== id) {
                    return it;
                }

                const updated = { ...it };

                if (field === 'quantity') {
                    let num = Math.floor(Number(value) || 0);

                    if (min !== undefined) {
                        num = Math.max(min, num);
                    }

                    if (max !== undefined) {
                        num = Math.min(max, num);
                    }

                    updated.quantity = num;
                }
                else if (field === 'unitPrice') {
                    let num = Number(value) || 0;

                    if (min !== undefined) {
                        num = Math.max(min, num);
                    }

                    if (max !== undefined) {
                        num = Math.min(max, num);
                    }

                    updated.unitPrice = Number(num.toFixed(2));
                }
                else {
                    updated[field] = value;
                }

                return updated;
            });

            return { ...prev, items: handleRecalcTotals(items) };
        });
    }

    function handleBlurNormalizeNumber(id: Guid, field: 'quantity' | 'unitPrice') {
        setFormData(prev => {
            const items = prev.items!.map(it => {
                if (it.quoteItemId !== id) {
                    return it;
                }

                const updated = { ...it };

                if (field === 'quantity') {
                    updated.quantity = Math.max(0, Math.floor(updated.quantity ?? 0));
                }

                if (field === 'unitPrice') {
                    updated.unitPrice = Number((updated.unitPrice ?? 0).toFixed(2));
                }

                updated.totalPrice = Number(((updated.quantity ?? 0) * (updated.unitPrice ?? 0)).toFixed(2));

                return updated;
            });

            return { ...prev, items };
        });
    }

    useEffect(() => {
        if (formData.items) {
            setFormData(prev => ({
                ...prev,
                items: handleRecalcTotals(prev.items!)
            }));
        }
    }, []);

    function handleGetGrandTotal() {
        return (formData.items ?? []).reduce((sum, it) => sum + (it.totalPrice ?? 0), 0).toFixed(2);
    }

    return (
        <section className={styles.container}>
            <div className={styles.header}>
                <span className={styles.title}>Itens do orçamento <span className={styles.obligatory}>*</span></span>

                <Button
                    label={LABEL_BTN_ADD}
                    handleFunction={handleAddItem}
                    isDisabled={!editing}
                    icon_feather={<Icon icon='plus-circle' size='small' />}
                    style={{ fontSize: '0.85rem' }}
                />
            </div>

            <div className={styles.tableWrapper}>
                <table className={styles.table}>
                    <thead>
                        <tr>
                            <th>Descrição</th>
                            <th>Quantidade</th>
                            <th>Valor unitário</th>
                            <th>Total</th>
                            <th>Ações</th>
                        </tr>
                    </thead>

                    <tbody>
                        {
                            formData.items?.map(item => (
                                <tr key={item.quoteItemId?.toString()}>
                                    <td>
                                        <input
                                            type='text'
                                            className={styles.input}
                                            value={item.description ?? ''}
                                            onChange={(e) => handleUpdateItem(item.quoteItemId!, 'description', e.target.value)}
                                            disabled={!editing}
                                            placeholder='Descrição'
                                        />
                                    </td>

                                    <td>
                                        <input
                                            type='number'
                                            step={1}
                                            className={styles.input}
                                            value={item.quantity ?? 0}
                                            onChange={(e) => handleUpdateItem(item.quoteItemId!, 'quantity', e.target.value, 0, 9999)}
                                            onBlur={() => handleBlurNormalizeNumber(item.quoteItemId!, 'quantity')}
                                            disabled={!editing}
                                        />
                                    </td>

                                    <td>
                                        <input
                                            type='number'
                                            step={0.01}
                                            className={styles.input}
                                            value={item.unitPrice ?? 0}
                                            onChange={(e) => handleUpdateItem(item.quoteItemId!, 'unitPrice', e.target.value, 0, 999999)}
                                            onBlur={() => handleBlurNormalizeNumber(item.quoteItemId!, 'unitPrice')}
                                            disabled={!editing}
                                        />
                                    </td>

                                    <td className={styles.total}>
                                        R$ {(item.totalPrice ?? 0).toFixed(2)}
                                    </td>

                                    <td>
                                        <button
                                            type='button'
                                            onClick={() => handleRemoveItem(item.quoteItemId!)}
                                            disabled={!editing}
                                            className={styles.removeButton}
                                        >
                                            Remover
                                        </button>
                                    </td>
                                </tr>
                            ))
                        }

                        {
                            (formData?.items?.length === 0 || !formData?.items) && (
                                <tr>
                                    <td colSpan={5} className={styles.empty}>
                                        Nenhum item adicionado. Clique em <strong onClick={handleAddItem}>{LABEL_BTN_ADD.toLowerCase()}</strong>.
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>

            <div className={styles.footer}>
                Total geral: <strong>R$ {handleGetGrandTotal()}</strong>
            </div>
        </section>
    )
}