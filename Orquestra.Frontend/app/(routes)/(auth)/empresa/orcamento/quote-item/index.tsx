import { iQuote, iQuoteItem } from '@/app/api/consts/quote';
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

    function recalcTotals(items: iQuoteItem[]) {
        return items.map((it) => ({
            ...it,
            totalPrice: Number(((it.quantity ?? 0) * (it.unitPrice ?? 0)).toFixed(2))
        }));
    }

    function handleAddItem() {
        const newItem: iQuoteItem = {
            title: '',
            description: '',
            quantity: 1,
            unitPrice: 0,
            totalPrice: 0,
        };

        setFormData(prev => ({
            ...prev,
            items: recalcTotals([...(prev.items ?? []), newItem])
        }));
    }

    function handleRemoveItem(id: Guid) {
        setFormData(prev => ({
            ...prev,
            items: recalcTotals(prev.items!.filter(i => i.quoteItemId !== id))
        }));
    }

    function handleUpdateItem(id: Guid, field: keyof Omit<iQuoteItem, 'quoteItemId' | 'totalPrice'>, value: any) {
        setFormData(prev => {
            const items = prev.items!.map(it => {
                if (it.quoteItemId !== id) return it;

                const updated = { ...it };

                if (field === 'quantity') updated.quantity = Number(value) || 0;
                else if (field === 'unitPrice') updated.unitPrice = Number(value) || 0;
                else updated[field] = value;

                return updated;
            });

            return { ...prev, items: recalcTotals(items) };
        });
    }

    function handleBlurNormalizeNumber(id: Guid, field: 'quantity' | 'unitPrice') {
        setFormData(prev => {
            const items = prev.items!.map(it => {
                if (it.quoteItemId !== id) return it;

                const updated = { ...it };

                if (field === 'quantity')
                    updated.quantity = Math.max(0, Math.floor(updated.quantity ?? 0));

                if (field === 'unitPrice')
                    updated.unitPrice = Number((updated.unitPrice ?? 0).toFixed(2));

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
                items: recalcTotals(prev.items!)
            }));
        }
    }, []);

    function getGrandTotal() {
        return (formData.items ?? []).reduce((sum, it) => sum + (it.totalPrice ?? 0), 0).toFixed(2);
    }

    return (
        <section className={styles.container}>
            <div className={styles.header}>
                <h3>Itens</h3>
                <button
                    type='button'
                    onClick={handleAddItem}
                    disabled={!editing}
                    className={styles.addButton}
                >
                    {LABEL_BTN_ADD}
                </button>
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
                                            onChange={(e) =>
                                                handleUpdateItem(item.quoteItemId!, 'description', e.target.value)
                                            }
                                            disabled={!editing}
                                            placeholder='Descrição'
                                        />
                                    </td>

                                    <td>
                                        <input
                                            type='number'
                                            min={0}
                                            step={1}
                                            className={styles.input}
                                            value={item.quantity ?? 0}
                                            onChange={(e) =>
                                                handleUpdateItem(item.quoteItemId!, 'quantity', e.target.value)
                                            }
                                            onBlur={() =>
                                                handleBlurNormalizeNumber(item.quoteItemId!, 'quantity')
                                            }
                                            disabled={!editing}
                                        />
                                    </td>

                                    <td>
                                        <input
                                            type='number'
                                            min={0}
                                            step={0.01}
                                            className={styles.input}
                                            value={item.unitPrice ?? 0}
                                            onChange={(e) =>
                                                handleUpdateItem(item.quoteItemId!, 'unitPrice', e.target.value)
                                            }
                                            onBlur={() =>
                                                handleBlurNormalizeNumber(item.quoteItemId!, 'unitPrice')
                                            }
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
                Total geral: <strong>R$ {getGrandTotal()}</strong>
            </div>
        </section>
    )
}