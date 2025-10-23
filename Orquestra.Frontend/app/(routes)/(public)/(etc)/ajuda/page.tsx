'use client';
import ImgMeditation from '@/app/assets/webp/meditation.webp';
import SYSTEM from '@/app/consts/system';
import useTitle from '@/app/hooks/useTitle';
import Image from 'next/image';
import AjudaListCards from './components/list-cards';
import AjudaSearchInput from './components/seach-input';
import styles from './page.module.scss';

export interface iAjudaTopico {
    topic: string;
    description: string;
    items: iAjudaTopicoItem[];
}

export interface iAjudaTopicoItem {
    title: string;
    description: string;
}

export const HELP_TOPICS = [
    {
        topic: 'Criar conta e acesso',
        description: `Saiba como criar sua conta, fazer login e manter a segurança de acesso ao ${SYSTEM.NAME}.`,
        items: [
            {
                title: 'Como criar uma conta?',
                description: 'Clique em "Crie uma conta agora mesmo", preencha os dados e confirme o e-mail enviado para ativar sua conta.'
            },
            {
                title: 'Esqueci minha senha',
                description: 'Na tela de login, clique em "Esqueci minha senha", informe seu e-mail e siga o link recebido para redefinir a senha.'
            },
            {
                title: 'Acessar em múltiplos dispositivos',
                description: 'Você pode usar seu e-mail e senha tanto no computador quanto no celular; o acesso é multi-dispositivo.'
            },
            {
                title: 'Posso usar um e-mail já cadastrado?',
                description: 'Não é possível criar uma conta com um e-mail já utilizado por você ou outro usuário.'
            }
        ]
    },
    {
        topic: 'Clientes',
        description: 'Gerencie o cadastro, histórico e relacionamento com seus clientes.',
        items: [
            {
                title: 'Cadastrar clientes manualmente',
                description: 'No módulo "Clientes", clique em "Adicionar" e preencha nome, telefone, e-mail e demais informações relevantes.'
            },
            {
                title: 'Consultar histórico do cliente',
                description: 'No perfil do cliente, visualize agendamentos, pagamentos e observações registradas.'
            },
            {
                title: 'Importar lista de clientes',
                description: 'Faça upload de um arquivo CSV para importar múltiplos clientes simultaneamente.'
            }
        ]
    },
    {
        topic: 'Agendamentos',
        description: 'Organize seus horários, serviços e notificações automáticas.',
        items: [
            {
                title: 'Marcar um agendamento',
                description: 'No módulo "Agendamentos", clique em uma data na agenda e preencha os dados do horário, profissional e cliente.'
            },
            {
                title: 'Cancelar ou reagendar',
                description: 'Abra o agendamento e selecione "Cancelar" ou "Reagendar". Cliente e profissional serão notificados automaticamente.'
            },
            {
                title: 'Lembretes automáticos',
                description: `O ${SYSTEM.NAME} envia notificações por e-mail e WhatsApp para evitar esquecimentos.`
            }
        ]
    },
    {
        topic: 'Financeiro',
        description: 'Acompanhe pagamentos, relatórios e o fluxo de caixa da sua empresa.',
        items: [
            {
                title: 'Relatórios financeiros',
                description: 'Visualize relatórios de entrada e saída por período no módulo "Financeiro" para manter o controle do seu negócio.'
            },
            {
                title: 'Controle de recebimentos',
                description: `Monitore todos os pagamentos realizados pelo ${SYSTEM.NAME}, incluindo pendentes e em processamento.`
            }
        ]
    },
    {
        topic: 'Membros e equipe',
        description: 'Gerencie os profissionais da equipe e seus níveis de acesso.',
        items: [
            {
                title: 'Cadastrar novos membros',
                description: 'No módulo "Membros", clique em "Adicionar" e defina permissões e serviços vinculados.'
            },
            {
                title: 'Definir agenda de profissionais',
                description: 'Cada membro pode ter seus horários personalizados no calendário.'
            },
            {
                title: 'Gerenciar permissões de acesso',
                description: 'Controle quem pode acessar informações financeiras, agendamentos, configurações e outros módulos.'
            },
            {
                title: 'Apenas um administrador pode gerir os membros?',
                description: 'Sim, somente administradores podem realizar alterações na equipe.'
            }
        ]
    },
    {
        topic: 'LGPD e privacidade',
        description: `Saiba como o ${SYSTEM.NAME} protege os dados de clientes, profissionais e usuários, garantindo segurança, transparência e conformidade com a LGPD.`,
        items: [
            {
                title: 'Armazenamento seguro',
                description: 'Todos os dados são armazenados de forma segura, com criptografia em trânsito e em repouso, além de backups automáticos regulares.'
            },
            {
                title: 'Exclusão de dados pessoais',
                description: 'É possível remover todas as informações de um cliente ou usuário mediante solicitação, atendendo aos direitos previstos na LGPD.'
            },
            {
                title: 'Consentimento do usuário',
                description: 'O cadastro de clientes e usuários só é realizado com consentimento explícito para coleta e uso de dados pessoais.'
            },
            {
                title: 'Compartilhamento de dados',
                description: 'Os dados não são compartilhados com terceiros sem autorização, exceto quando exigido por lei ou para prestação de serviços contratados.'
            },
            {
                title: 'Acesso restrito',
                description: 'Somente usuários autorizados dentro da plataforma podem acessar informações sensíveis, de acordo com suas permissões.'
            },
            {
                title: 'Monitoramento e auditoria',
                description: 'Registramos acessos e alterações em dados importantes para garantir rastreabilidade e prevenção de uso indevido.'
            },
            {
                title: 'Atualizações de privacidade',
                description: 'Mantemos nossos procedimentos e políticas sempre atualizados para garantir conformidade com a LGPD e melhores práticas de segurança.'
            }
        ]
    },
    {
        topic: 'Módulos',
        description: `Conheça os módulos disponíveis no ${SYSTEM.NAME} e como eles podem ser personalizados conforme a necessidade da sua empresa.`,
        items: [
            {
                title: 'Como funcionam os módulos?',
                description: 'Os módulos do sistema representam áreas específicas da sua empresa — como Agenda, Finanças, Clientes e Relatórios — e podem ser ativados ou desativados conforme o tipo de serviço que você oferece.'
            },
            {
                title: 'Quem pode gerenciar os módulos?',
                description: 'Apenas administradores têm permissão para atribuir ou remover módulos de outros membros da equipe. Isso pode ser feito acessando "Gerenciar empresa" (/empresa/gerenciar) ou pela tela de "Plano e faturas" (/empresa/uso-e-plano).'
            },
            {
                title: 'Posso ativar ou desativar módulos?',
                description: 'Sim. Em "Gerenciar empresa", você pode escolher exatamente quais módulos estarão ativos na sua conta. Desative o que não usa para deixar o ambiente mais limpo e ágil.'
            },
            {
                title: 'Novos módulos',
                description: `A equipe do ${SYSTEM.NAME} adiciona novos módulos regularmente, trazendo ferramentas para automatizar ainda mais o seu fluxo de trabalho. Fique de olho nas atualizações!`
            }
        ]
    },
    {
        topic: 'Planos',
        description: `Saiba quais são os planos disponíveis no ${SYSTEM.NAME} e o que cada um oferece.`,
        items: [
            {
                title: 'Período de teste gratuito',
                description: 'Toda empresa começa com um plano gratuito de 14 dias, com acesso total a todos os recursos da plataforma. Nenhum cartão é necessário para iniciar o teste.'
            },
            {
                title: 'Plano básico',
                description: 'Ideal para pequenas equipes ou profissionais autônomos. Inclui agendamentos ilimitados, cadastro de clientes e relatórios básicos. Custo acessível e sem taxas escondidas.'
            },
            {
                title: 'Plano premium',
                description: `Feito para quem quer o máximo do ${SYSTEM.NAME}. Traz dashboards avançados, automações, módulos financeiros completos, suporte prioritário, etc.`
            },
            {
                title: 'Alterar ou cancelar plano',
                description: 'O plano pode ser alterado ou cancelado a qualquer momento em "Plano e faturas" (/empresa/uso-e-plano). Mudanças de plano são aplicadas imediatamente após a confirmação.'
            }
        ]
    },
    {
        topic: 'Segurança',
        description: 'Saiba como protegemos seus dados e garantimos acesso seguro à plataforma.',
        items: [
            {
                title: 'Autenticação segura',
                description: 'Todas as senhas são criptografadas e protegidas com os padrões de segurança mais modernos.'
            },
            {
                title: 'Proteção de dados sensíveis',
                description: 'Informações financeiras e pessoais são armazenadas de forma totalmente segura, com criptografia e políticas de acesso restrito.'
            },
            {
                title: 'Monitoramento constante',
                description: 'Nossos sistemas de segurança monitoram continuamente atividades suspeitas para prevenir qualquer tentativa de acesso não autorizado.'
            },
            {
                title: 'Controle de acesso por funções',
                description: 'Cada usuário tem permissões específicas, garantindo que apenas pessoas autorizadas possam acessar informações críticas.'
            },
            {
                title: 'Backups regulares',
                description: 'Realizamos backups automáticos periódicos para proteger seus dados contra perdas acidentais ou falhas técnicas.'
            },
            {
                title: 'Atualizações de segurança',
                description: 'Nossa plataforma recebe atualizações constantes para corrigir vulnerabilidades e manter a proteção contra ameaças emergentes.'
            }
        ]
    },
    {
        topic: 'Termos de uso',
        description: `Saiba o que você pode e não pode fazer ao usar o ${SYSTEM.NAME}, e entenda suas responsabilidades como usuário.`,
        items: [
            {
                title: 'Aceite dos termos',
                description: `Ao criar uma conta e utilizar o ${SYSTEM.NAME}, você concorda integralmente com estes Termos de Uso e com nossa Política de Privacidade.`
            },
            {
                title: 'Responsabilidade do usuário',
                description: 'Você é responsável pelas informações cadastradas, bem como pelo uso adequado da plataforma e dos dados de seus clientes.'
            },
            {
                title: 'Uso comercial permitido',
                description: `O ${SYSTEM.NAME} pode ser utilizado para fins profissionais e comerciais, desde que respeite as leis vigentes e os direitos de terceiros.`
            },
            {
                title: 'Limitação de responsabilidade',
                description: `O ${SYSTEM.NAME} não se responsabiliza por perdas, danos ou prejuízos decorrentes de uso indevido da plataforma ou falhas externas de rede.`
            },
            {
                title: 'Encerramento de conta',
                description: `O usuário pode solicitar o encerramento da conta a qualquer momento. A plataforma também pode suspender ou excluir contas em caso de uso indevido ou violação dos termos. Para isso, contate o e-mail "${SYSTEM.EMAIL_SUPPORT}".`
            },
            {
                title: 'Alterações nos termos',
                description: 'Os Termos de Uso podem ser atualizados periodicamente. Alterações relevantes serão comunicadas aos usuários por e-mail ou dentro da própria plataforma.'
            }
        ]
    },
    {
        topic: 'Faturas',
        description: `Entenda como funcionam as faturas no ${SYSTEM.NAME} e o que acontece após a aquisição de um novo plano.`,
        items: [
            {
                title: 'Criação de fatura',
                description: `Sempre que um novo plano é adquirido, o ${SYSTEM.NAME} gera automaticamente uma nova fatura vinculada à empresa. Essa fatura precisa ser paga para que o plano seja efetivamente ativado.`
            },
            {
                title: 'Ativação do plano',
                description: `O plano adquirido só entra em vigor após o pagamento da fatura correspondente. Antes disso, a empresa permanece com a situação <b>pendente de pagamento</b> e o plano anterior (se existir) é desconsiderado.`
            },
            {
                title: 'Histórico de faturas',
                description: `Todas as faturas emitidas ficam registradas na aba "Histórico de faturas" dentro de "Plano e faturas" (/empresa/uso-e-plano). Lá é possível visualizar datas, valores, status e descrições detalhadas.`
            },
            {
                title: 'Situação da fatura',
                description: `As faturas podem ter três situações principais: <b>Pendente</b> (aguardando pagamento), <b>Aprovada</b> (pagamento confirmado) e <b>Cancelada</b> (fatura invalidada manualmente ou substituída por outra).`
            },
            {
                title: 'Cancelamento automático',
                description: `Ao adquirir um novo plano antes de pagar o atual, a fatura anterior é automaticamente cancelada, garantindo que apenas a fatura mais recente permaneça ativa.`
            }
        ]
    }
] as iAjudaTopico[];

export default function Ajuda() {

    useTitle('Central de ajuda');

    return (
        <section className={styles.main}>
            <div className={styles.hero}>
                <span>Central de ajuda</span>

                <div className={SYSTEM.ANIMATE_PULSE_INFINITE}>
                    <Image src={ImgMeditation} alt='' priority={true} />
                </div>
            </div>

            <AjudaSearchInput keySearch='' hasAltStyle={true} />
            <AjudaListCards topics={[...HELP_TOPICS].sort((a, b) => a.topic.localeCompare(b.topic))} />
        </section>
    )
}