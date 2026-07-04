import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import DoctorDashboardView from '../views/DoctorDashboardView.vue'
import AdminDashboardView from '../views/AdminDashboardView.vue'
import PatientListView from '../views/PatientListView.vue'

import AppointmentsView from '../views/AppointmentsView.vue'
import AppointmentDetailView from '../views/AppointmentDetailView.vue'
import UsersView from '../views/UsersView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { 
      path: '/', 
      redirect: '/login' 
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
      path: '/dashboard-medico', 
      name: 'dashboard-medico', 
      component: DoctorDashboardView 
    },
    { 
      path: '/dashboard-admin', 
      name: 'dashboard-admin', 
      component: AdminDashboardView 
    },
    { 
      path: '/pacientes', 
      name: 'pacientes', 
      component: PatientListView 
    },
    { 
      path: '/citas', 
      name: 'citas', 
      component: AppointmentsView 
    },
    { 
      path: '/citas/detalle', 
      name: 'detalle-cita', 
      component: AppointmentDetailView 
    },
    { 
      path: '/usuarios', 
      name: 'usuarios', 
      component: UsersView 
    }
  ]
})

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('auth') === 'true'
  
  if (!to.meta.public && !isAuthenticated) {
    next('/login')
  } else if (to.meta.public && isAuthenticated) {
    next('/dashboard-medico')
  } else {
    next()
  }
})

export default router