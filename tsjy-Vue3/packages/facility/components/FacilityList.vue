<script setup>
import { ElMessageBox, useLocale } from 'element-plus'
import { useFacilities } from '../composables/facilities'
import { ref, watch } from 'vue'

// region 组件
const props = defineProps(['multipleSelect'])
const emits = defineEmits(['change'])
const { t } = useLocale()
// endregion

// region 设备数据
const { facilities, loadingFacilities } = useFacilities()
const selectedFacilities = ref([])
watch(facilities, () => {
  if (facilities.value && facilities.value.length) {
    if (props['multipleSelect']) {
      let count
      if (facilities.value.length >= 3) {
        count = 3
      } else {
        count = facilities.value.length
      }
      selectedFacilities.value = []
      for (let i = 0; i < count; i++) {
        selectedFacilities.value.push(facilities.value[i])
      }
      emits('change', JSON.parse(JSON.stringify(selectedFacilities.value)))
    } else {
      selectedFacilities.value = [facilities.value[0]]
      emits('change', JSON.parse(JSON.stringify(selectedFacilities.value[0])))
    }
  }
})
const isActive = (id) => {
  const index = selectedFacilities.value.findIndex(
    (facility) => facility['id'] === id,
  )
  return index >= 0
}
const onFacilityClick = async (facility) => {
  if (!facility['isEnabled']) {
    try {
      await ElMessageBox.confirm(t('el.facility.facilityList.isDisabled'), '', {
        type: 'warning',
      })
    } catch {
      return
    }
  }
  if (props['multipleSelect']) {
    // 多选
    const index = selectedFacilities.value.findIndex(
      (item) => item['id'] === facility['id'],
    )
    if (index >= 0) {
      if (selectedFacilities.value.length > 1) {
        // 至少选择一个设备，大于一个元素时才删除
        selectedFacilities.value.splice(index, 1)
      }
    } else {
      selectedFacilities.value.push(facility)
    }
    emits('change', JSON.parse(JSON.stringify(selectedFacilities.value)))
  } else {
    selectedFacilities.value = [facility]
    emits('change', JSON.parse(JSON.stringify(selectedFacilities.value[0])))
  }
}
// endregion
</script>

<template>
  <ElScrollbar class="scrollbar" view-style="height: 100%">
    <div class="facility-container" v-loading="loadingFacilities">
      <div
        class="facility"
        v-for="facility of facilities"
        :key="facility['id']"
        :class="{ 'is-active': isActive(facility['id']) }"
        @click="onFacilityClick(facility)"
      >
        <ElText truncated class="name">
          {{ facility['name'] }}
        </ElText>
        <ElText truncated size="small" class="serial-number">
          {{
            facility['serialNumber']
              ? facility['serialNumber']
              : t('el.facility.facilityList.emptySerialNumber')
          }}
        </ElText>
      </div>
    </div>
  </ElScrollbar>
</template>

<style scoped lang="scss">
.scrollbar {
  height: min-content;
  min-height: 6rem;

  .facility-container {
    height: 100%;
    display: flex;
    grid-gap: 1rem;

    > .facility {
      flex-shrink: 0;
      display: flex;
      flex-flow: column nowrap;
      width: 16rem;
      padding: 0.5rem 1rem;
      margin-bottom: 1rem;
      border-radius: var(--el-border-radius-base);
      border: 1px solid var(--el-border-color);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--el-color-primary);
      }

      > .name {
        width: 100%;
      }

      > .serial-number {
        width: 100%;
        color: var(--el-text-color-secondary);
        min-height: 1.6rem;
      }

      &.is-active {
        background-color: var(--el-color-primary-light-9);
      }
    }
  }
}
</style>
