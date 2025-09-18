'use client';
import SvgOne from '@/app/assets/svg/one.svg';
import SvgTwo from '@/app/assets/svg/two.svg';
import CalendarSimple from '@/app/components/calendar/simple';
import CardSimple from '@/app/components/card/simple';
import SYSTEM from '@/app/consts/system';
import styles from './index.module.scss';

export default function CardCalendar() {

    return (
        <section className={styles.wrapper}>
            <CalendarSimple />

            <div className={styles.panel}>
                <div className={styles.panelInner}>
                    <div className={styles.panelHeader}>
                        <h2>Comece a usar o {SYSTEM.NAME}</h2>
                    </div>

                    <div className={styles.steps}>
                        <CardSimple
                            img={SvgOne}
                            title='Adicione seus primeiros contatos'
                            description='Você precisa de contatos para criar uma campanha. Crie seu banco de dados de contatos ou adicione os destinatários da sua primeira campanha.'
                            buttonLabel='Importar seus contatos'
                            buttonFunction={() => alert('xd')}
                        />

                        <CardSimple
                            img={SvgTwo}
                            title='Crie sua primeira campanha'
                            description='É hora de ser criativo e criar uma campanha. Precisa de inspiração? Escolha um modelo e use nosso assistente de redação com IA.'
                            buttonLabel='Crie sua primeira campanha'
                            buttonFunction={() => alert('xd')}
                        />
                    </div>
                </div>
            </div>
        </section>
    )
}