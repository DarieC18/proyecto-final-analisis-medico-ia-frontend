import { createRouter, createWebHistory } from 'vue-router'
import LandingView from '../views/LandingView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
import ResetPasswordView from '../views/ResetPasswordView.vue'
import ConfirmAccountView from '../views/ConfirmAccountView.vue'
import DoctorDashboardView from '../views/DoctorDashboardView.vue'
import AdminDashboardView from '../views/AdminDashboardView.vue'
import PatientListView from '../views/PatientListView.vue'
import AppointmentsView from '../views/AppointmentsView.vue'
import AppointmentDetailView from '../views/AppointmentDetailView.vue'
import UsersView from '../views/UsersView.vue'
import AuditLogView from '../views/AuditLogView.vue'
import ProfileView from '../views/ProfileView.vue'
import AlertsView from '../views/AlertsView.vue'
import RecommendationsView from '../views/RecommendationsView.vue'
import MedicalDocumentsView from '../views/MedicalDocumentsView.vue'
import ReportsView from '../views/ReportsView.vue'
import AiChatView from '../views/AiChatView.vue'
import PatientProfileView from '../views/PatientProfileView.vue'
import PatientAppointmentsView from '../views/PatientAppointmentsView.vue'
import PatientHistoryView from '../views/PatientHistoryView.vue'
import PatientRecommendationsView from '../views/PatientRecommendationsView.vue'
import PatientDocumentsView from '../views/PatientDocumentsView.vue'
import PatientResultsView from '../views/PatientResultsView.vue'
import NurseFollowUpView from '../views/NurseFollowUpView.vue'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'landing',
      component: LandingView,
      meta: { public: true }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { public: true }
    },
    {
      path: '/registro',
      name: 'registro',
      component: RegisterView,
      meta: { public: true }
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPasswordView,
      meta: { public: true }
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: ResetPasswordView,
      meta: { public: true }
    },
    {
      path: '/confirm-account',
      name: 'confirm-account',
      component: ConfirmAccountView,
      meta: { public: true }
    },
    {
      path: '/dashboard-medico',
      name: 'dashboard-medico',
      component: DoctorDashboardView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/dashboard-admin',
      name: 'dashboard-admin',
      component: AdminDashboardView,
      meta: { roles: ['Administrator'] }
    },
    {
      path: '/pacientes',
      name: 'pacientes',
      component: PatientListView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/citas',
      name: 'citas',
      component: AppointmentsView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/citas/:id',
      name: 'detalle-cita',
      component: AppointmentDetailView,
      props: true,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      component: UsersView,
      meta: { roles: ['Administrator'] }
    },
    {
      path: '/auditoria',
      name: 'auditoria',
      component: AuditLogView,
      meta: { roles: ['Administrator'] }
    },
    {
      path: '/perfil',
      name: 'perfil',
      component: ProfileView,
      meta: { roles: ['Doctor', 'Nurse', 'Administrator'] }
    },
    {
      path: '/alertas',
      name: 'alertas',
      component: AlertsView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/recomendaciones',
      name: 'recomendaciones',
      component: RecommendationsView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/documentos',
      name: 'documentos',
      component: MedicalDocumentsView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/reportes',
      name: 'reportes',
      component: ReportsView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/chat-ia',
      name: 'chat-ia',
      component: AiChatView,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/seguimiento-pacientes',
      name: 'seguimiento-pacientes',
      component: NurseFollowUpView,
      meta: { roles: ['Nurse'] }
    },

    {
      path: '/portal/perfil',
      name: 'portal-perfil',
      component: PatientProfileView,
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/citas',
      name: 'portal-citas',
      component: PatientAppointmentsView,
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/historial',
      name: 'portal-historial',
      component: PatientHistoryView,
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/recomendaciones',
      name: 'portal-recomendaciones',
      component: PatientRecommendationsView,
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/documentos',
      name: 'portal-documentos',
      component: PatientDocumentsView,
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/resultados',
      name: 'portal-resultados',
      component: PatientResultsView,
      meta: { roles: ['Patient'] }
    },

  ]
})

router.beforeEach((to, from) => {
  const isAuthenticated = !!localStorage.getItem('accessToken')
  const userData = localStorage.getItem('user')
  const user = userData ? JSON.parse(userData) : null
  const roles = user?.roles || []

  if (!to.meta.public && !isAuthenticated) {
    return '/login'
  }

  const homeRoute = roles.includes('Administrator') ? '/dashboard-admin'
    : roles.includes('Doctor') || roles.includes('Nurse') ? '/dashboard-medico'
    : roles.includes('Patient') ? '/portal/perfil'
    : '/perfil'

  if (to.meta.public && isAuthenticated) {
    return homeRoute
  }

  if (!to.meta.public && to.meta.roles && isAuthenticated) {
    const hasRole = to.meta.roles.some(r => roles.includes(r))
    if (!hasRole) {
      return homeRoute
    }
  }
})

export default router
