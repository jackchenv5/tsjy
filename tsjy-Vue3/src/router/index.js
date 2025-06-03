import { createRouter, createWebHashHistory } from 'vue-router'
import { useCommonRouter } from '#/common/router'
import { commonRoutes } from '#/common/routes'
import { mqttRoutes } from '#/mqtt/routes/index.js'
import { variableRoutes } from '#/variable/routes/index.js'
import { shiftRoutes } from '#/shift/routes/index.js'
import { facilityRoutes } from '#/facility/routes/index.js'
import { facilityStatusRoutes } from '#/facility-status/routes/index.js'

const tsjyRoutes = [
  {
    path: '/tsjy-facility-status',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'status',
        component: () => import('@/views/StatusSummary.vue'),
      },
      {
        path: 'status-binding',
        component: () => import('@/views/StatusBinding.vue'),
      },
    ],
  },
  {
    path: '/tsjy-facility-showall',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'showall',
        component: () => import('@/views/FacilityShowAll.vue'),
      }
    ],
  },
  {
    path: '/tsjy-alarm',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'definition',
        component: () => import('@/views/AlarmDefinition.vue'),
      },
      {
        path: 'current-alarms',
        component: () => import('@/views/CurrentAlarm.vue'),
      },
      {
        path: 'alarms-history',
        component: () => import('@/views/AlarmHistory.vue'),
      },
    ],
  },
  {
    path: '/tsjy-motor',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'setting',
        component: () => import('@/views/MotorSetting.vue'),
      },
      {
        path: 'data',
        component: () => import('@/views/MotorData.vue'),
      },
    ],
  },
  {
    path: '/tsjy-production',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'setting',
        component: () => import('@/views/ProductionSetting.vue'),
      },
      {
        path: 'history',
        component: () => import('@/views/ProductionHistory.vue'),
      },
    ],
  },
  {
    path: '/tsjy-craft',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'binding',
        component: () => import('@/views/CraftBinding.vue'),
      },
      {
        path: 'history',
        component: () => import('@/views/CraftHistory.vue'),
      },
    ],
  },
  {
    path: '/tsjy-part',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'list',
        component: () => import('@/views/PartList.vue'),
      },
      {
        path: 'binding',
        component: () => import('@/views/PartBinding.vue'),
      },
      {
        path: 'history',
        component: () => import('@/views/PartMaintainHistory.vue'),
      },
    ],
  },
  {
    path: '/hy-roll-diameter',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: '',
        component: () => import('@/views/DiameterData.vue'), // 主界面
        children: [
          {
            path: '',
            redirect: '/hy-roll-diameter/realtime'
          },
          {
            path: 'realtime',
            component: () => import('@/components/RollDiameterRealtimeData.vue'),
          },
          {
            path: 'history',
            component: () => import('@/components/RollDiameterHistoryData.vue'),
          },
        ],
      },
    ],
  },
  
  {
    path: '/hy-PrintLength',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: '',
        component: () => import('@/views/PrintLength.vue'), // 主界面
        children: [
          {
            path: '',
            redirect: '/hy-PrintLength/realtime' // 默认重定向到实时数据
          },
          {
            path: 'realtime',
            component: () => import('@/components/PrintLengthRealtime.vue'),
          },
          {
            path: 'history',
            component: () => import('@/components/PrintLengthHistory.vue'),
          },
        ],
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
    ...facilityStatusRoutes,
    ...tsjyRoutes,
  ],
})

useCommonRouter(router)

export default router
