import { createRouter, createWebHashHistory } from 'vue-router'
import { useCommonRouter } from '#/common/router'
import { commonRoutes } from '#/common/routes'
import { mqttRoutes } from '#/mqtt/routes/index.js'
import { variableRoutes } from '#/variable/routes/index.js'
import { shiftRoutes } from '#/shift/routes/index.js'
// import { facilityRoutes } from '#/facility/routes/index.js'
// import { facilityStatusRoutes } from '#/facility-status/routes/index.js'

const facilityRoutes = [
  {
    path: '/facilitys',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'all',
        component: () => import('@/views/facilitys/All.vue'),
      },
      {
        path: 'parameters',
        component: () => import('@/views/facilitys/Parameters.vue'),
      },
      {
        path: 'monitor',
        component: () => import('@/views/facilitys/MotorMonitor.vue'),
      },
      {
        path: 'oee',
        component: () => import('@/views/facilitys/Oee.vue'),
      },
      {
        path: 'alarm',
        component: () => import('@/views/facilitys/Aalarm.vue'),
      },
      {
        path: 'history',
        component: () => import('@/views/facilitys/History.vue'),
      },
    ],
  },
]

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    ...commonRoutes,
    ...mqttRoutes,
    ...variableRoutes,
    ...shiftRoutes,
    ...facilityRoutes,
    // ...facilityStatusRoutes,
    // ...tsjyRoutes,
  ],
})

useCommonRouter(router)

export default router
