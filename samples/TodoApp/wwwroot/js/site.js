// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

const text = document.querySelector('#new-item-text');
const button = document.querySelector('#add-new-item');

text.addEventListener('input', () => {
    if (text.value.length === 0) {
        button.setAttribute('disabled', '');
    } else {
        button.removeAttribute('disabled');
    }
});
