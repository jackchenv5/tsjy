<script setup>
import { onBeforeUnmount, onMounted, ref, toValue, watch } from 'vue'
import { useLocale } from 'element-plus'
import { useResize } from '#/common/composables/resize.js'
import { getCurrentShiftStatusApi } from '#/facility-status/apis/facilityStatus.js'
import { getSawingMachineStatusApi } from '@/apis/sawingMachineStatus.js'
import DataItem from '@/components/DataItem.vue'
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
onMounted(async () => {
  try {
    await getCurrentShiftStatus()
    await getSawingMachineStatus()
    startInterval();
  } catch (error) {
    console.error('初始化失败',error);
  }
});
onBeforeUnmount(() => {
  stopInterval()
})
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
  //对设备的唯一表示ID进行判断，非控才能进行下一步
  if (!props.facility['id']) {
    return
  }
  loading.value = true
  try {
    const { data } = await getCurrentShiftStatusApi(props.facility['id'])
    currentStatus.value = data['currentStatus']
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
    await getSawingMachineStatus()
  }, 2000)
}
const stopInterval = () => {
  if (interval) {
    clearInterval(interval)
  }
}
const getStatusString = () => {
  switch (currentStatus.value) {
    case 0:
      return t('el.facilityStatus.stoppedData.invalid')
    case 1:
      return t('el.facilityStatus.stoppedData.running')
    case 2:
      return t('el.facilityStatus.stoppedData.standby')
    case 3:
      return t('el.facilityStatus.stoppedData.stopped')
    case 4:
      return t('el.facilityStatus.stoppedData.error')
  }
}

// region 图片
const imgUrl = ref(new URL('@/assets/JYKJ.png', import.meta.url).href)
const getImageUrl = (facilityId) => {
  // 从 localStorage 获取设备的图片路径
  const customImgUrl = localStorage.getItem(`facility-${facilityId}`);
  return customImgUrl || imgUrl.value;
};
// endregion

// region 切割机特定状态
const sawingMachineStatus = ref({
  autoStepNum: 0,
  lineVelocity: 0,
  newLineLength: 0,
  currentFeedSpeed: 0,
  totalCutHeight: 0,
  cutHeight: 0,
  tensionValueTop: 0,
  tensionValueBottom: 0,
  mainWheelDiameter: 0,
  customerCode: '-',
  materialSpecification: '-',
  cutPercentage: 0,
  totalCutTime: '00:00:00',
  remainingTime: '00:00:00',
})
const loadingSawingMachineStatus = ref(false)
const getSawingMachineStatus = async () => {
  if (!props.facility['id']) {
    return
  }
  loadingSawingMachineStatus.value = true
  try {
    const { data } = await getSawingMachineStatusApi(props.facility['id'])
    sawingMachineStatus.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingSawingMachineStatus.value = false
  }
}
// endregion

// region 数据标签样式

const statusColor = ref()
const statusBgColor = ref()
const statusMarkColor = ref()

watch(currentStatus, () => {
  updateStatusColors()
})

const updateStatusColors = () => {
  switch (currentStatus.value) {
    case 1:
      statusColor.value = toValue(useCssVar('--green-7'))
      statusBgColor.value = toValue(useCssVar('--green-2'))
      statusMarkColor.value = toValue(useCssVar('--green-6'))
      break
    case 2:
      statusColor.value = toValue(useCssVar('--blue-7'))
      statusBgColor.value = toValue(useCssVar('--blue-2'))
      statusMarkColor.value = toValue(useCssVar('--blue-6'))
      break
    case 3:
      statusColor.value = toValue(useCssVar('--gold-7'))
      statusBgColor.value = toValue(useCssVar('--gold-2'))
      statusMarkColor.value = toValue(useCssVar('--gold-6'))
      break
    case 4:
      statusColor.value = toValue(useCssVar('--red-7'))
      statusBgColor.value = toValue(useCssVar('--red-2'))
      statusMarkColor.value = toValue(useCssVar('--red-6'))
      break
    default:
      statusColor.value = toValue(useCssVar('--gray-8'))
      statusBgColor.value = toValue(useCssVar('--gray-2'))
      statusMarkColor.value = toValue(useCssVar('--gray-8'))
      break
  }
}

useDark({
  onChanged: () => {
    updateStatusColors()
  },
})
// endregion

// region 设备详情
const showDetail = ref(false)
const showDetailClick = () => {
  showDetail.value = true
}
// endregion
</script>

<template>
  <div class="facility-status">
    <div class="header">
      <span>{{ facility['name'] }}</span>
      <span class="status" :class="`status-${currentStatus}`">{{ getStatusString() }}</span>
      <ElButton type="primary" link size="small" @click="showDetailClick">
        详细信息
      </ElButton>
    </div>
    <div class="content" :class="breakPoint">
      <div class="data">
        <DataItem label="当前段号" :content="sawingMachineStatus.autoStepNum">
        </DataItem>
        <DataItem
          label="切线速度"
          :content="sawingMachineStatus.lineVelocity.toFixed(3)"
        >
        </DataItem>
        <DataItem
          label="切割百分比"
          :content="sawingMachineStatus.cutPercentage.toFixed(3)"
        >
        </DataItem>

        <DataItem
          label="切割总时间"
          :content="sawingMachineStatus.totalCutTime"
        >
        </DataItem>
        <DataItem label="剩余时间" :content="sawingMachineStatus.remainingTime">
        </DataItem>
      </div>
    </div>

    <ElDialog
      v-model="showDetail"
      :title="facility['name'] + ' 详情'"
      class="dialog"
      :class="breakPoint"
    >
      <div class="detail">
        <div class="img">
          <img :src="getImageUrl(props.facility.id)" alt="弹窗中设备图片" />
        </div>
        <div class="data">
          <DataItem label="当前段号" :content="sawingMachineStatus.autoStepNum">
          </DataItem>
          <DataItem
            label="切线速度"
            :content="sawingMachineStatus.lineVelocity.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="切割百分比"
            :content="sawingMachineStatus.cutPercentage"
          >
          </DataItem>
          <DataItem
            label="切割总时间"
            :content="sawingMachineStatus.totalCutTime"
          >
          </DataItem>
          <DataItem
            label="剩余时间"
            :content="sawingMachineStatus.remainingTime"
          >
          </DataItem>
          <DataItem
            label="新线进给量"
            :content="sawingMachineStatus.newLineLength.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="进给速度"
            :content="sawingMachineStatus.currentFeedSpeed.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="切割总高度"
            :content="sawingMachineStatus.totalCutHeight.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="已切割高度"
            :content="sawingMachineStatus.cutHeight.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="上摆杆张力设定值"
            :content="sawingMachineStatus.tensionValueTop.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="下摆杆张力设定值"
            :content="sawingMachineStatus.tensionValueBottom.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="槽轮直径"
            :content="sawingMachineStatus.mainWheelDiameter.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="客户代码"
            :content="sawingMachineStatus.customerCode"
          >
          </DataItem>
          <DataItem
            label="切割材料规格"
            :content="sawingMachineStatus.materialSpecification"
          >
          </DataItem>
        </div>
      </div>
    </ElDialog>
  </div>
</template>

<style scoped lang="scss">
.facility-status {
  max-width: 240px; // 设置最大宽度
  margin: 0 auto; // 居中显示
  //外框的边框粗细，颜色，圆角大小
  border: 3px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 2rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 3px solid var(--el-border-color);

    > .status {
      padding: 0 1rem;
      border-radius: 0.6rem;
      color: white;
    }

    > .status-0 {
      //状态为0表示无效，为灰色表示
      background-color: var(--gray-6);
    }

    > .status-1 {
      //状态为1表示运行，为绿色表示
      background-color: var(--green-6);
    }

    > .status-2 {
      //状态为2表示备用，为蓝色表示
      background-color: var(--blue-6);
    }

    > .status-3 {
      //状态为3表示停止，为黄色表示
      background-color: var(--yellow-6);
    }

    > .status-4 {
      //状态为4表示错误，为红色表示
      background-color: var(--red-6);
    }
  }

  > .content {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    padding: 1rem;

    > .data {
      flex: 1;
      width: 12rem;
      display: flex;
      flex-flow: column;
      grid-gap: 1rem;
    }
  }
}
</style>

<style lang="scss">
.facility-status {
  .dialog {
    width: 80rem;

    .detail {
      display: flex;
      flex-flow: row;
      grid-gap: 1rem;

      > .data {
        flex: 1;
        display: grid;
        grid-template-columns: repeat(2, minmax(12rem, 1fr));
        grid-gap: 1rem;
      }

      > .img {
        flex: 4;
        object-fit: contain;

        > img {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }
      }
    }

    &.xs,
    &.sm {
      width: 32rem;

      .detail {
        > .img {
          display: none;
        }
      }
    }

    &.md {
      width: 72rem;
    }
  }
}
</style>
