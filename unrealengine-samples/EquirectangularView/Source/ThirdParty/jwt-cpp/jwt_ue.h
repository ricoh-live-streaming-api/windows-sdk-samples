// Copyright 2022 RICOH Company, Ltd. All rights reserved.

#pragma once

#define UI UI_ST
#define JWT_DISABLE_PICOJSON

#ifdef verify
#undef verify
#endif

#include "traits/nlohmann-json/traits.h"
