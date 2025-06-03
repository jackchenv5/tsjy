<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

// region 选中设备
const currentFacility = ref()
const router = useRouter()
const facilityChange = (facility) => {
  currentFacility.value = facility
  const currentPath = router.currentRoute.value.path
  if (!currentPath.startsWith('/hy-PrintLength')) {
    router.push('/hy-PrintLength/realtime') // 默认跳转到实时数据
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>印刷长度分析</template>
    <template #default>
      <FacilityList @change="facilityChange"></FacilityList>
      <div class="container" v-if="currentFacility">
        <div class="data-container">
          <router-view :facility-id="currentFacility.id"></router-view>
        </div>
      </div>
      <ElEmpty v-else description="请先选择设备"></ElEmpty>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.container {
  flex: 1;
  display: flex;
  flex-flow: column;
  overflow: auto;

  .data-container {
    flex: 1;
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
  }
}
</style>