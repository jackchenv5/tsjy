<script setup>
import { useResize } from '#/common/composables/resize'
import { dayjs, useLocale } from 'element-plus'
import * as echarts from 'echarts'
import { onMounted, ref, toValue, watch } from 'vue'
import { useCssVar, useDark } from '@vueuse/core'
import duration from 'dayjs/plugin/duration'
import { vElementSize } from '@vueuse/components'

// region 组件
const { breakPoint } = useResize()
const { t, lang } = useLocale()
const model = defineModel()
const props = defineProps({
  statusData: {
    type: Array,
    require: true,
  },
})
onMounted(() => {
  initChart()
  updateChart()
})
dayjs.extend(duration)
// endregion

// region 图表
const chartRef = ref()
let chart
const valueFormatter = (seconds) => {
  return dayjs.duration(seconds * 1000).format('HH:mm:ss')
}
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'item',
  },
  legend: {
    bottom: '10',
    textStyle: {
      color: 'inherit',
    },
    padding: 0,
  },
  series: [
    {
      type: 'pie',
      radius: ['30%', '50%'],
      center: ['50%', '40%'],
      bottom: '10',
      top: '10',
      label: {
        show: true,
        position: 'outside',
        formatter: '{d} %',
        color: 'inherit',
        fontSize: '16px',
      },
      data: [
        { value: 0, name: '运行' },
        { value: 0, name: '待机' },
        { value: 0, name: '停机' },
        { value: 0, name: '故障' },
      ],
      tooltip: {
        valueFormatter: valueFormatter,
        confine: true,
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
const updateChartLanguage = () => {
  chartOptions.series[0].data[0].name = t(
    'el.facilityStatus.stoppedData.running',
  )
  chartOptions.series[0].data[1].name = t(
    'el.facilityStatus.stoppedData.standby',
  )
  chartOptions.series[0].data[2].name = t(
    'el.facilityStatus.stoppedData.stopped',
  )
  chartOptions.series[0].data[3].name = t('el.facilityStatus.stoppedData.error')

  updateChart()
}
watch(
  lang,
  () => {
    updateChartLanguage()
  },
  {
    immediate: true,
  },
)
// endregion

// region 数据
const processStatus = () => {
  let runningSeconds = 0
  let standbySeconds = 0
  let stoppedSeconds = 0
  let errorSeconds = 0
  for (const record of props.statusData) {
    if (record['status'] === 'Running') {
      runningSeconds += record['duration']
    }
    if (record['status'] === 'Standby') {
      standbySeconds += record['duration']
    }
    if (record['status'] === 'Stopped') {
      stoppedSeconds += record['duration']
    }
    if (record['status'] === 'Error') {
      errorSeconds += record['duration']
    }
  }
  updateChartData(runningSeconds, standbySeconds, stoppedSeconds, errorSeconds)
}
watch(model, async (value) => {
  if (value) {
    processStatus()
  }
  model.value = false
})
// endregion
</script>

<template>
  <div class="status-compare-chart-root" :class="breakPoint">
    <div class="header">
      {{ t('el.facilityStatus.stoppedData.statusCompare') }}
    </div>
    <div
      class="chart"
      ref="chartRef"
      v-loading="model"
      v-element-size="onResize"
    ></div>
  </div>
</template>

<style scoped lang="scss">
.status-compare-chart-root {
  flex: 1;
  display: flex;
  flex-flow: column;
  width: 0;
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  &.xs {
    width: auto;
  }

  > .header {
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-bottom: 1px solid var(--el-border-color);
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
  }

  > .chart {
    flex: 1 0;
    height: 0;
  }
}
</style>
