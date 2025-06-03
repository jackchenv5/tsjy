<script setup>
import { dayjs, useLocale } from 'element-plus'
import { useResize } from '#/common/composables/resize'
import { vElementSize } from '@vueuse/components'
import { onBeforeUnmount, onMounted, ref, toValue } from 'vue'
import { getCurrentShiftStatusApi } from '../apis/facilityStatus'
import duration from 'dayjs/plugin/duration'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'

// region 组件
const { t } = useLocale()
const { breakPoint } = useResize()
const props = defineProps({
  facility: {
    type: Object,
    require: true,
  },
})
onMounted(() => {
  startInterval()
  initChart()
  updateChart()
})
onBeforeUnmount(() => {
  stopInterval()
})
dayjs.extend(duration)
// endregion

// region 图表
const valueFormatter = (seconds) => {
  return dayjs.duration(seconds * 1000).format('HH:mm:ss')
}
const chartRef = ref()
let chart
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    show: true,
    trigger: 'item',
  },
  legend: {
    bottom: '0',
    textStyle: {
      color: 'inherit',
    },
    padding: 0,
    selectedMode: false,
  },
  series: [
    {
      type: 'pie',
      radius: ['30%', '50%'],
      center: ['50%', '50%'],
      label: {
        show: true,
        position: 'outside',
        formatter: '{d} %',
        color: 'inherit',
        fontSize: '16px',
      },
      data: [
        { value: 1048, name: '运行' },
        { value: 735, name: '待机' },
        { value: 580, name: '停机' },
        { value: 484, name: '故障' },
      ],
      tooltip: {
        valueFormatter: valueFormatter,
      },
    },
  ],
}
const initChart = () => {
  chart = echarts.init(chartRef.value)
}
const updateChart = () => {
  if (chart) {
    chart.setOption(chartOptions, true)
  }
}
const updateChartColor = () => {
  chartOptions.textStyle.color = toValue(useCssVar('--gray-9'))
  chartOptions.color = [
    toValue(useCssVar('--green-6')),
    toValue(useCssVar('--primary-6')),
    toValue(useCssVar('--gold-6')),
    toValue(useCssVar('--red-6')),
  ]
  updateChart()
}
useDark({
  onChanged() {
    if (chart) {
      updateChartColor()
    }
  },
})
const updateChartData = (
  runningSeconds,
  standbySeconds,
  stoppedSeconds,
  errorSeconds,
) => {
  chartOptions.series[0].data[0].value = runningSeconds
  chartOptions.series[0].data[1].value = standbySeconds
  chartOptions.series[0].data[2].value = stoppedSeconds
  chartOptions.series[0].data[3].value = errorSeconds

  updateChart()
}

const onResize = () => {
  chart.resize()
}
// endregion

// region 数据
/*
    Invalid = 0,
    Running = 1,
    Standby = 2,
    Stopped = 3,
    Error = 4
 */
const currentStatus = ref(0)
const loading = ref(false)
const getCurrentShiftStatus = async () => {
  if (!props.facility.id) {
    return
  }
  loading.value = true
  try {
    const { data } = await getCurrentShiftStatusApi(props.facility.id)
    currentStatus.value = data['currentStatus']
    updateTotal(
      data['runningSeconds'] +
        data['standbySeconds'] +
        data['stoppedSeconds'] +
        data['errorSeconds'],
    )
    updateChartData(
      data['runningSeconds'],
      data['standbySeconds'],
      data['stoppedSeconds'],
      data['errorSeconds'],
    )
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}
let interval
const startInterval = () => {
  stopInterval()
  interval = setInterval(async () => {
    await getCurrentShiftStatus()
  }, 5000)
}
const stopInterval = () => {
  if (interval) {
    clearInterval(interval)
  }
}
const total = ref()
const updateTotal = (totalSeconds) => {
  const duration = dayjs.duration(totalSeconds * 1000).format('HH:mm:ss')
  total.value = `${t('el.facilityStatus.statusSummary.totalColon')}${duration}`
}
// endregion
</script>

<template>
  <div class="facility-status-root">
    <div class="header-container" :class="`status-${currentStatus}`">
      <span>{{ facility['name'] }}</span>
    </div>
    <div
      class="content-container"
      :class="breakPoint"
      v-element-size="onResize"
    >
      <div class="title-container">
        <div>
          {{ t('el.facilityStatus.statusSummary.statusSummary') }}
        </div>
        <ElTag size="small">
          {{ total }}
        </ElTag>
      </div>
      <div class="chart" ref="chartRef"></div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.facility-status-root {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  > .header-container {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 1px solid var(--el-border-color);

    &.status-1 {
      background-color: var(--green-6);
    }

    &.status-2 {
      background-color: var(--primary-6);
    }

    &.status-3 {
      background-color: var(--gold-6);
    }

    &.status-4 {
      background-color: var(--red-6);
    }
  }

  > .content-container {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    padding: 1rem;
    height: 24rem;
    position: relative;

    > .title-container {
      position: absolute;
      display: flex;
      grid-gap: 1rem;
      align-items: center;
    }

    > .chart {
      flex: 1;
    }
  }
}
</style>
