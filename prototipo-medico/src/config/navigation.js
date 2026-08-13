import { markRaw } from 'vue'
import {
  IconAi,
  IconAlert,
  IconAppointment,
  IconAudit,
  IconClinical,
  IconDashboard,
  IconDocument,
  IconHistory,
  IconPatients,
  IconRecommendation,
  IconReport,
  IconRoles,
  IconSparkles,
  IconUser
} from '@/lib/icons'

/**
 * Fuente única de la navegación por rol.
 *
 * Antes esta información estaba triplicada: `meta.roles` en el router,
 * los `v-if="auth.hasRole(...)"` del Navbar, y el ternario de ruta home
 * copiado entre el Navbar y el guard. Las tres podían divergir — y de hecho
 * divergían (el sidebar previo omitía /seguimiento-pacientes).
 *
 * Reparto de responsabilidades:
 *   - `meta.roles` del router  -> autoridad de SEGURIDAD
 *   - este archivo             -> autoridad de PRESENTACIÓN
 *
 * `routeNames` existe para marcar el ítem activo en rutas anidadas: la ruta
 * `detalle-cita` (/citas/:id) debe iluminar "Citas".
 */

export const navSections = [
  {
    id: 'clinico',
    label: 'Clínico',
    roles: ['Doctor', 'Nurse'],
    items: [
      {
        to: '/dashboard-medico',
        label: 'Dashboard',
        icon: markRaw(IconDashboard),
        routeNames: ['dashboard-medico']
      },
      {
        to: '/pacientes',
        label: 'Pacientes',
        icon: markRaw(IconPatients),
        routeNames: ['pacientes']
      },
      {
        to: '/citas',
        label: 'Citas',
        icon: markRaw(IconAppointment),
        routeNames: ['citas', 'detalle-cita']
      },
      {
        to: '/seguimiento-pacientes',
        label: 'Seguimiento',
        icon: markRaw(IconClinical),
        roles: ['Nurse'],
        routeNames: ['seguimiento-pacientes']
      },
      {
        to: '/alertas',
        label: 'Alertas',
        icon: markRaw(IconAlert),
        routeNames: ['alertas']
      },
      {
        to: '/recomendaciones',
        label: 'Recomendaciones',
        icon: markRaw(IconRecommendation),
        routeNames: ['recomendaciones']
      },
      {
        to: '/documentos',
        label: 'Documentos',
        icon: markRaw(IconDocument),
        routeNames: ['documentos']
      },
      {
        to: '/reportes',
        label: 'Reportes',
        icon: markRaw(IconReport),
        routeNames: ['reportes']
      },
      {
        to: '/chat-ia',
        label: 'Asistente IA',
        icon: markRaw(IconAi),
        routeNames: ['chat-ia']
      }
    ]
  },
  {
    id: 'admin',
    label: 'Administración',
    roles: ['Administrator'],
    items: [
      {
        to: '/dashboard-admin',
        label: 'Panel',
        icon: markRaw(IconDashboard),
        routeNames: ['dashboard-admin']
      },
      {
        to: '/usuarios',
        label: 'Usuarios',
        icon: markRaw(IconRoles),
        routeNames: ['usuarios']
      },
      {
        to: '/auditoria',
        label: 'Auditoría',
        icon: markRaw(IconAudit),
        routeNames: ['auditoria']
      }
    ]
  },
  {
    id: 'portal',
    label: 'Mi salud',
    roles: ['Patient'],
    items: [
      {
        to: '/portal/perfil',
        label: 'Mi perfil',
        icon: markRaw(IconUser),
        routeNames: ['portal-perfil']
      },
      {
        to: '/portal/citas',
        label: 'Mis citas',
        icon: markRaw(IconAppointment),
        routeNames: ['portal-citas']
      },
      {
        to: '/portal/historial',
        label: 'Historial',
        icon: markRaw(IconHistory),
        routeNames: ['portal-historial']
      },
      {
        to: '/portal/resultados',
        label: 'Resultados',
        icon: markRaw(IconSparkles),
        routeNames: ['portal-resultados']
      },
      {
        to: '/portal/recomendaciones',
        label: 'Recomendaciones',
        icon: markRaw(IconRecommendation),
        routeNames: ['portal-recomendaciones']
      },
      {
        to: '/portal/documentos',
        label: 'Documentos',
        icon: markRaw(IconDocument),
        routeNames: ['portal-documentos']
      }
    ]
  },
  {
    id: 'cuenta',
    label: 'Cuenta',
    roles: ['Doctor', 'Nurse', 'Administrator'],
    items: [
      {
        to: '/perfil',
        label: 'Mi perfil',
        icon: markRaw(IconUser),
        routeNames: ['perfil']
      }
    ]
  }
]

const matchesRoles = (allowed, roles) => !allowed || allowed.some((r) => roles.includes(r))

/** Secciones e ítems visibles para un conjunto de roles. Sin secciones vacías. */
export function navForRoles(roles = []) {
  return navSections
    .filter((section) => matchesRoles(section.roles, roles))
    .map((section) => ({
      ...section,
      items: section.items.filter((item) => matchesRoles(item.roles, roles))
    }))
    .filter((section) => section.items.length > 0)
}

/** Ruta de inicio según rol. La consumen el guard del router y la marca. */
export function homeRouteForRoles(roles = []) {
  if (roles.includes('Administrator')) return '/dashboard-admin'
  if (roles.includes('Doctor') || roles.includes('Nurse')) return '/dashboard-medico'
  if (roles.includes('Patient')) return '/portal/perfil'
  return null
}

/**
 * Aviso en desarrollo si la navegación y el router se desincronizan.
 * Es la red que evita que vuelvan a divergir como hacían las tres copias
 * anteriores. No se ejecuta en producción.
 */
export function assertNavMatchesRoutes(router) {
  if (!import.meta.env.DEV) return

  const byName = new Map(router.getRoutes().map((r) => [r.name, r]))

  for (const section of navSections) {
    for (const item of section.items) {
      const resolved = router.resolve(item.to)
      if (!resolved.matched.length) {
        console.warn(`[navigation] "${item.to}" no resuelve a ninguna ruta.`)
        continue
      }

      const allowed = item.roles ?? section.roles
      for (const name of item.routeNames ?? []) {
        const route = byName.get(name)
        if (!route) {
          console.warn(`[navigation] routeName "${name}" no existe en el router.`)
          continue
        }
        const metaRoles = route.meta?.roles
        if (allowed && metaRoles && !allowed.every((r) => metaRoles.includes(r))) {
          console.warn(
            `[navigation] "${item.label}" se muestra a [${allowed}] pero la ruta "${name}" ` +
              `sólo permite [${metaRoles}].`
          )
        }
      }
    }
  }
}
