<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import MaintainRecord from '../components/MaintainRecord.vue'

// region 组件
const { t } = useLocale()
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityMaintain.maintainHistory.maintainHistory') }}
    </template>
    <template #default>
      <div class="maintain-history-root">
        <MaintainRecord class="maintain-record"></MaintainRecord>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.maintain-history-root {
  flex: 1;
  display: flex;
  flex-flow: row nowrap;
  overflow: auto;
  grid-gap: 1rem;

  > .maintain-record {
    flex: 2;
    overflow: auto;
  }
}
</style>
