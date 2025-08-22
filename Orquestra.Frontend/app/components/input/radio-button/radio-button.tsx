import styles from './radio-button.module.scss';

export interface iRadioButtonOption {
    value: number;
    label: string;
}

interface iParams {
    title?: string;
    options: iRadioButtonOption[];
    selectedId: number | null;
    onChange: (id: number) => void;
}

export default function RadioButton({
    title,
    options,
    selectedId,
    onChange
}: iParams) {
    return (
        <div className={styles.main}>
            {title && <span className={styles.title}>{title}</span>}

            <div className={styles.radioButtonGroup}>
                {
                    options?.map((option) => (
                        <label key={option.value}>
                            <input
                                type='radio'
                                id={option.value.toString()}
                                value={option.value}
                                checked={option.value === selectedId}
                                onChange={() => onChange(option.value)}
                            />

                            {option.label}
                        </label>
                    ))
                }
            </div>
        </div>
    )
}