<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import { useResize } from '#/common/composables/resize'
import CurrentShift from '#/shift/components/CurrentShift.vue'
import { useFacilities } from '#/facility/composables/facilities'
import FacilityStatus from '../components/FacilityStatus.vue'

// region 组件
const { t } = useLocale()
const { breakPoint } = useResize()
// endregion

// region 设备
const { enabledFacilities, loadingFacilities } = useFacilities()
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      <div class="header-container">
        <div>
          {{ t('el.facilityStatus.statusSummary.statusSummary') }}
        </div>
        <CurrentShift></CurrentShift>
      </div>
    </template>
    <template #default>
      <div
        class="content-container"
        :class="breakPoint"
        v-loading="loadingFacilities"
      >
        <FacilityStatus
          v-for="(facility, index) of enabledFacilities"
          :key="index"
          :facility="facility"
        >
        </FacilityStatus>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.header-container {
  display: flex;
  grid-gap: 1rem;
  align-items: center;
}

.content-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(32rem, 1fr));
  grid-gap: 1rem;

  &.lg {
    > *:only-child {
      max-width: 50%;
    }
  }

  &.xl {
    > *:only-child {
      max-width: 40%;
    }
  }

  &.xxl {
    > *:only-child {
      max-width: 40%;
    }
  }
}
</style>
