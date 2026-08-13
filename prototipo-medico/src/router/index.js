import { createRouter, createWebHistory } from 'vue-router'
import { assertNavMatchesRoutes, homeRouteForRoles } from '@/config/navigation'

// Las públicas se importan de forma estática: son la primera pantalla que ve
// cualquiera y no conviene meterles un salto de red extra. El resto va lazy.
import LandingView from '@/views/LandingView.vue'
import LoginView from '@/views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  scrollBehavior: (to, from, saved) => saved ?? { top: 0 },
  routes: [
    // `bareLayout`: la Landing trae su propio header (marca, login, toggle de
    // tema); el PublicLayout no debe superponerle su botón flotante.
    { path: '/', name: 'landing', component: LandingView, meta: { public: true, bareLayout: true } },
    { path: '/login', name: 'login', component: LoginView, meta: { public: true } },
    {
      path: '/registro',
      name: 'registro',
      component: () => import('@/views/RegisterView.vue'),
      meta: { public: true }
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: () => import('@/views/ForgotPasswordView.vue'),
      meta: { public: true }
    },
    {
      path: '/reset-password',
      name: 'reset-password',
      component: () => import('@/views/ResetPasswordView.vue'),
      meta: { public: true }
    },
    {
      path: '/confirm-account',
      name: 'confirm-account',
      component: () => import('@/views/ConfirmAccountView.vue'),
      meta: { public: true }
    },

    // --- Clínico (Doctor / Nurse) ---
    {
      path: '/dashboard-medico',
      name: 'dashboard-medico',
      component: () => import('@/views/DoctorDashboardView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/pacientes',
      name: 'pacientes',
      component: () => import('@/views/PatientListView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/citas',
      name: 'citas',
      component: () => import('@/views/AppointmentsView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/citas/:id',
      name: 'detalle-cita',
      component: () => import('@/views/AppointmentDetailView.vue'),
      props: true,
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/alertas',
      name: 'alertas',
      component: () => import('@/views/AlertsView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/recomendaciones',
      name: 'recomendaciones',
      component: () => import('@/views/RecommendationsView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/documentos',
      name: 'documentos',
      component: () => import('@/views/MedicalDocumentsView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/reportes',
      name: 'reportes',
      component: () => import('@/views/ReportsView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/chat-ia',
      name: 'chat-ia',
      component: () => import('@/views/AiChatView.vue'),
      meta: { roles: ['Doctor', 'Nurse'] }
    },
    {
      path: '/seguimiento-pacientes',
      name: 'seguimiento-pacientes',
      component: () => import('@/views/NurseFollowUpView.vue'),
      meta: { roles: ['Nurse'] }
    },

    // --- Administración ---
    {
      path: '/dashboard-admin',
      name: 'dashboard-admin',
      component: () => import('@/views/AdminDashboardView.vue'),
      meta: { roles: ['Administrator'] }
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      component: () => import('@/views/UsersView.vue'),
      meta: { roles: ['Administrator'] }
    },
    {
      path: '/auditoria',
      name: 'auditoria',
      component: () => import('@/views/AuditLogView.vue'),
      meta: { roles: ['Administrator'] }
    },

    // --- Cuenta ---
    {
      path: '/perfil',
      name: 'perfil',
      component: () => import('@/views/ProfileView.vue'),
      meta: { roles: ['Doctor', 'Nurse', 'Administrator'] }
    },

    // --- Portal del paciente ---
    {
      path: '/portal/perfil',
      name: 'portal-perfil',
      component: () => import('@/views/PatientProfileView.vue'),
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/citas',
      name: 'portal-citas',
      component: () => import('@/views/PatientAppointmentsView.vue'),
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/historial',
      name: 'portal-historial',
      component: () => import('@/views/PatientHistoryView.vue'),
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/recomendaciones',
      name: 'portal-recomendaciones',
      component: () => import('@/views/PatientRecommendationsView.vue'),
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/documentos',
      name: 'portal-documentos',
      component: () => import('@/views/PatientDocumentsView.vue'),
      meta: { roles: ['Patient'] }
    },
    {
      path: '/portal/resultados',
      name: 'portal-resultados',
      component: () => import('@/views/PatientResultsView.vue'),
      meta: { roles: ['Patient'] }
    },

    // Catálogo del sistema de diseño. Sólo existe en desarrollo: en producción
    // el array no lo incluye, así que ni siquiera se genera su chunk.
    ...(import.meta.env.DEV
      ? [
          {
            path: '/ui-kit',
            name: 'ui-kit',
            component: () => import('@/views/UiKitView.vue'),
            meta: { public: true, noAuthRedirect: true }
          }
        ]
      : []),

    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/NotFoundView.vue'),
      meta: { public: true, noAuthRedirect: true }
    }
  ]
})

router.beforeEach((to) => {
  const isAuthenticated = !!localStorage.getItem('accessToken')
  const userData = localStorage.getItem('user')

  let user = null
  try {
    user = userData ? JSON.parse(userData) : null
  } catch {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
    return '/login'
  }

  const roles = user?.roles?.filter(Boolean) || []

  if (!to.meta.public && !isAuthenticated) {
    return '/login'
  }

  // Fuente compartida con el sidebar y la marca: antes este ternario estaba
  // duplicado aquí y en el Navbar, y podían discrepar.
  const homeRoute = homeRouteForRoles(roles)

  // `noAuthRedirect` marca las rutas públicas que NO deben rebotar a un usuario
  // autenticado hacia su home: si escribe mal una URL tiene que ver el 404, no
  // un redirect silencioso, y el catálogo de UI debe abrirse con sesión activa.
  if (to.meta.public && isAuthenticated && !to.meta.noAuthRedirect) {
    if (homeRoute) return homeRoute
    // Sesión con roles inválidos: limpiar y dejar pasar a la página pública
    localStorage.removeItem('accessToken')
    localStorage.removeItem('user')
    return
  }

  if (!to.meta.public && to.meta.roles && isAuthenticated) {
    const hasRole = to.meta.roles.some((r) => roles.includes(r))
    if (!hasRole) {
      if (homeRoute) return homeRoute
      // Sin rol reconocido: limpiar sesión y redirigir a login
      localStorage.removeItem('accessToken')
      localStorage.removeItem('user')
      return '/login'
    }
  }
})

assertNavMatchesRoutes(router)

export default router
