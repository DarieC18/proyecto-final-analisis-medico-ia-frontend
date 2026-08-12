/**
 * Barrel de iconos de dominio.
 *
 * Re-export NOMBRADO de lucide-vue-next. Es importante que sea nombrado:
 * lucide declara `sideEffects: false` y publica ESM, así que Rollup hace
 * tree-shaking perfecto — pero sólo con imports nombrados. Un
 * `import * as icons from 'lucide-vue-next'` o una resolución por string desde
 * el objeto módulo mete las ~1500 iconos en el bundle.
 *
 * Los alias de dominio dan un único punto donde cambiar un icono en toda la
 * app: si mañana "Documento" deja de ser FileText, se cambia aquí y ya.
 */

export {
  // --- Dominio clínico ---
  Stethoscope as IconClinical,
  HeartPulse as IconVitals,
  Thermometer as IconTemperature,
  Heart as IconHeartRate,
  Droplet as IconBlood,
  Gauge as IconOxygen,
  Wind as IconBreathing,
  Activity as IconPulse,
  Brain as IconAi,
  Bot as IconBot,
  Microscope as IconLab,
  Pill as IconMedication,
  Hospital as IconHospital,
  ClipboardList as IconRecord,
  ClipboardPlus as IconSymptoms,
  Lightbulb as IconRecommendation,
  Bell as IconAlert,
  TriangleAlert as IconWarning,

  // --- Personas ---
  User as IconUser,
  Users as IconPatients,
  UserPlus as IconUserAdd,
  UserCog as IconRoles,
  ShieldCheck as IconSecurity,
  ShieldAlert as IconAudit,

  // --- Documentos y archivos ---
  FileText as IconDocument,
  FileImage as IconImageFile,
  FileType2 as IconPdf,
  Paperclip as IconAttachment,
  SquarePen as IconNotes,
  NotebookPen as IconNotebook,
  Upload as IconUpload,
  Download as IconDownload,
  ScrollText as IconReport,

  // --- Navegación y acciones ---
  LayoutDashboard as IconDashboard,
  CalendarDays as IconAppointment,
  CalendarPlus as IconAppointmentAdd,
  MessageSquare as IconChat,
  SendHorizontal as IconSend,
  Search as IconSearch,
  ListFilter as IconFilter,
  Plus as IconAdd,
  Pencil as IconEdit,
  Trash2 as IconDelete,
  Eye as IconView,
  Save as IconSave,
  X as IconClose,
  Check as IconCheck,
  CircleCheck as IconSuccess,
  CircleX as IconError,
  CircleAlert as IconAttention,
  Info as IconInfo,
  ArrowLeft as IconBack,
  ArrowRight as IconForward,
  ArrowUpRight as IconExternal,
  ChevronDown as IconChevronDown,
  ChevronRight as IconChevronRight,
  RefreshCw as IconRefresh,
  History as IconHistory,
  ChartColumn as IconChart,
  Sparkles as IconSparkles,
  Zap as IconFast,

  // --- Cuenta y sesión ---
  IdCard as IconIdentification,
  KeyRound as IconPassword,
  Lock as IconLock,
  Mail as IconEmail,
  Phone as IconPhone,
  MapPin as IconAddress,
  Cake as IconBirthdate,
  Settings as IconSettings,
  LogOut as IconLogout,

  // --- Shell y tema ---
  Menu as IconMenu,
  Sun as IconLight,
  Moon as IconDark,
  Monitor as IconSystem
} from 'lucide-vue-next'
