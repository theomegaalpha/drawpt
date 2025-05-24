import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import TheEntry from '@/components/home/TheEntry.vue'

describe('TheEntry', () => {
  it('renders properly', () => {
    const wrapper = mount(TheEntry, { props: { msg: 'Welcome,' } })
    expect(wrapper.text()).toContain('Welcome,')
  })
})
