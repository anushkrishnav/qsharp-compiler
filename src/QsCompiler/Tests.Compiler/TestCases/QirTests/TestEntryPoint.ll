define { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* @Microsoft__Quantum__Testing__QIR__TestEntryPoint({ i64, i8* }* %arr, i8* %str, i8 %res, { i64, i64, i64 }* %range, i64 %cnt, i8 %b) #0 {
entry:
  %0 = getelementptr { i64, i8* }, { i64, i8* }* %arr, i64 0, i32 0
  %1 = getelementptr { i64, i8* }, { i64, i8* }* %arr, i64 0, i32 1
  %2 = load i64, i64* %0, align 4
  %3 = load i8*, i8** %1, align 8
  %4 = call %Array* @__quantum__rt__array_create_1d(i32 1, i64 %2)
  %5 = ptrtoint i8* %3 to i64
  %6 = sub i64 %2, 1
  br label %header__1

header__1:                                        ; preds = %exiting__1, %entry
  %7 = phi i64 [ 0, %entry ], [ %16, %exiting__1 ]
  %8 = icmp sle i64 %7, %6
  br i1 %8, label %body__1, label %exit__1

body__1:                                          ; preds = %header__1
  %9 = mul i64 %7, 1
  %10 = add i64 %5, %9
  %11 = inttoptr i64 %10 to i8*
  %12 = load i8, i8* %11, align 1
  %13 = bitcast i8 %12 to i2
  %14 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %4, i64 %7)
  %15 = bitcast i8* %14 to i2*
  store i2 %13, i2* %15, align 1
  br label %exiting__1

exiting__1:                                       ; preds = %body__1
  %16 = add i64 %7, 1
  br label %header__1

exit__1:                                          ; preds = %header__1
  %17 = call %String* @__quantum__rt__string_create(i32 0, i8* %str)
  %18 = load %Result*, %Result** @ResultZero, align 8
  %19 = load %Result*, %Result** @ResultOne, align 8
  %20 = icmp eq i8 %res, 0
  %21 = select i1 %20, %Result* %18, %Result* %19
  %22 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %range, i64 0, i32 0
  %23 = load i64, i64* %22, align 4
  %24 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %range, i64 0, i32 1
  %25 = load i64, i64* %24, align 4
  %26 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %range, i64 0, i32 2
  %27 = load i64, i64* %26, align 4
  %28 = load %Range, %Range* @EmptyRange, align 4
  %29 = insertvalue %Range %28, i64 %23, 0
  %30 = insertvalue %Range %29, i64 %25, 1
  %31 = insertvalue %Range %30, i64 %27, 2
  %32 = bitcast i8 %b to i1
  %33 = call %Tuple* @__quantum__rt__tuple_create(i64 ptrtoint ({ i64, i1 }* getelementptr ({ i64, i1 }, { i64, i1 }* null, i32 1) to i64))
  %34 = bitcast %Tuple* %33 to { i64, i1 }*
  %35 = getelementptr inbounds { i64, i1 }, { i64, i1 }* %34, i32 0, i32 0
  %36 = getelementptr inbounds { i64, i1 }, { i64, i1 }* %34, i32 0, i32 1
  store i64 %cnt, i64* %35, align 4
  store i1 %32, i1* %36, align 1
  %37 = call { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* @Microsoft__Quantum__Testing__QIR__TestEntryPoint__body(%Array* %4, %String* %17, %Result* %21, %Range %31, { i64, i1 }* %34)
  %38 = getelementptr inbounds { %Array*, %String*, %Result*, %Range, { i64, i1 }* }, { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37, i32 0, i32 0
  %39 = getelementptr inbounds { %Array*, %String*, %Result*, %Range, { i64, i1 }* }, { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37, i32 0, i32 1
  %40 = getelementptr inbounds { %Array*, %String*, %Result*, %Range, { i64, i1 }* }, { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37, i32 0, i32 2
  %41 = getelementptr inbounds { %Array*, %String*, %Result*, %Range, { i64, i1 }* }, { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37, i32 0, i32 3
  %42 = getelementptr inbounds { %Array*, %String*, %Result*, %Range, { i64, i1 }* }, { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37, i32 0, i32 4
  %43 = load %Array*, %Array** %38, align 8
  %44 = load %String*, %String** %39, align 8
  %45 = load %Result*, %Result** %40, align 8
  %46 = load %Range, %Range* %41, align 4
  %47 = load { i64, i1 }*, { i64, i1 }** %42, align 8
  %48 = call i64 @__quantum__rt__array_get_size_1d(%Array* %43)
  %49 = mul i64 %48, 1
  %50 = call i8* @__quantum__rt__heap_alloc(i64 %49)
  %51 = ptrtoint i8* %50 to i64
  %52 = sub i64 %48, 1
  br label %header__2

header__2:                                        ; preds = %exiting__2, %exit__1
  %53 = phi i64 [ 0, %exit__1 ], [ %62, %exiting__2 ]
  %54 = icmp sle i64 %53, %52
  br i1 %54, label %body__2, label %exit__2

body__2:                                          ; preds = %header__2
  %55 = mul i64 %53, 1
  %56 = add i64 %51, %55
  %57 = inttoptr i64 %56 to i8*
  %58 = call i8* @__quantum__rt__array_get_element_ptr_1d(%Array* %43, i64 %53)
  %59 = bitcast i8* %58 to i2*
  %60 = load i2, i2* %59, align 1
  %61 = bitcast i2 %60 to i8
  store i8 %61, i8* %57, align 1
  br label %exiting__2

exiting__2:                                       ; preds = %body__2
  %62 = add i64 %53, 1
  br label %header__2

exit__2:                                          ; preds = %header__2
  %63 = call i8* @__quantum__rt__heap_alloc(i64 ptrtoint ({ i64, i8* }* getelementptr ({ i64, i8* }, { i64, i8* }* null, i32 1) to i64))
  %64 = bitcast i8* %63 to { i64, i8* }*
  %65 = getelementptr { i64, i8* }, { i64, i8* }* %64, i64 0, i32 0
  store i64 %48, i64* %65, align 4
  %66 = getelementptr { i64, i8* }, { i64, i8* }* %64, i64 0, i32 1
  store i8* %50, i8** %66, align 8
  %67 = call i32 @__quantum__rt__string_get_length(%String* %44)
  %68 = sext i32 %67 to i64
  %69 = mul i64 8, %68
  %70 = call i8* @__quantum__rt__string_get_data(%String* %44)
  %71 = call i8* @__quantum__rt__heap_alloc(i64 %69)
  call void @llvm.memcpy.p0i8.p0i8.i64(i8* %71, i8* %70, i64 %69, i1 false)
  %72 = load %Result*, %Result** @ResultZero, align 8
  %73 = call i1 @__quantum__rt__result_equal(%Result* %45, %Result* %72)
  %74 = select i1 %73, i8 0, i8 -1
  %75 = extractvalue %Range %46, 0
  %76 = extractvalue %Range %46, 1
  %77 = extractvalue %Range %46, 2
  %78 = call i8* @__quantum__rt__heap_alloc(i64 mul nuw (i64 ptrtoint (i64* getelementptr (i64, i64* null, i32 1) to i64), i64 3))
  %79 = bitcast i8* %78 to { i64, i64, i64 }*
  %80 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %79, i64 0, i32 0
  store i64 %75, i64* %80, align 4
  %81 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %79, i64 0, i32 1
  store i64 %76, i64* %81, align 4
  %82 = getelementptr { i64, i64, i64 }, { i64, i64, i64 }* %79, i64 0, i32 2
  store i64 %77, i64* %82, align 4
  %83 = getelementptr inbounds { i64, i1 }, { i64, i1 }* %47, i32 0, i32 0
  %84 = getelementptr inbounds { i64, i1 }, { i64, i1 }* %47, i32 0, i32 1
  %85 = load i64, i64* %83, align 4
  %86 = load i1, i1* %84, align 1
  %87 = bitcast i1 %86 to i8
  %88 = call i8* @__quantum__rt__heap_alloc(i64 ptrtoint ({ i64, i8 }* getelementptr ({ i64, i8 }, { i64, i8 }* null, i32 1) to i64))
  %89 = bitcast i8* %88 to { i64, i8 }*
  %90 = getelementptr { i64, i8 }, { i64, i8 }* %89, i64 0, i32 0
  store i64 %85, i64* %90, align 4
  %91 = getelementptr { i64, i8 }, { i64, i8 }* %89, i64 0, i32 1
  store i8 %87, i8* %91, align 1
  %92 = call i8* @__quantum__rt__heap_alloc(i64 ptrtoint ({ { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* getelementptr ({ { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* null, i32 1) to i64))
  %93 = bitcast i8* %92 to { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }*
  %94 = getelementptr { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93, i64 0, i32 0
  store { i64, i8* }* %64, { i64, i8* }** %94, align 8
  %95 = getelementptr { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93, i64 0, i32 1
  store i8* %71, i8** %95, align 8
  %96 = getelementptr { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93, i64 0, i32 2
  store i8 %74, i8* %96, align 1
  %97 = getelementptr { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93, i64 0, i32 3
  store { i64, i64, i64 }* %79, { i64, i64, i64 }** %97, align 8
  %98 = getelementptr { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }, { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93, i64 0, i32 4
  store { i64, i8 }* %89, { i64, i8 }** %98, align 8
  call void @__quantum__rt__array_update_reference_count(%Array* %4, i64 -1)
  call void @__quantum__rt__string_update_reference_count(%String* %17, i64 -1)
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %33, i64 -1)
  call void @__quantum__rt__array_update_reference_count(%Array* %43, i64 -1)
  call void @__quantum__rt__string_update_reference_count(%String* %44, i64 -1)
  call void @__quantum__rt__result_update_reference_count(%Result* %45, i64 -1)
  %99 = bitcast { i64, i1 }* %47 to %Tuple*
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %99, i64 -1)
  %100 = bitcast { %Array*, %String*, %Result*, %Range, { i64, i1 }* }* %37 to %Tuple*
  call void @__quantum__rt__tuple_update_reference_count(%Tuple* %100, i64 -1)
  ret { { i64, i8* }*, i8*, i8, { i64, i64, i64 }*, { i64, i8 }* }* %93
}
