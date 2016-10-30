#!/usr/bin/env python
# -*- coding: utf-8 -*-

import os

PORT = int(os.environ.get('PORT', 5001))

DEBUG_ON = bool(os.environ.get('DEBUG', False))
