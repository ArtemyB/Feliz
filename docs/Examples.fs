﻿module Examples

open Elmish
open Feliz
open Fable.Core
open Zanaptak.TypedCssClasses
open System

type CounterRecordProps = { initial: int; show: bool }

type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.5/css/bulma.min.css", Naming.PascalCase>
type FA = CssClasses<"https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css", Naming.PascalCase>

type Msg =
    | Increment
    | Decrement

let keyedFragments state dispatch =
    React.keyedFragment(1, [
        Html.div [
            React.keyedFragment("hello", [
                Html.h1 "Hello"
                Html.div [ ]
                Html.div [ Html.h1 "More stuff" ]
                Html.div [ Html.h2 [ Html.strong "Bold" ] ]
                Html.ul [
                    Html.li [ Html.strong "Wow" ]
                    Html.li "So"
                    Html.li (Html.em "Lightweight")
                    Html.ol [
                        Html.li [ Html.strong "More" ]
                        Html.li [ ]
                        Html.li [ Html.none ]
                    ]
                ]
            ])
        ]
    ])

let inputs state dispatch =
    Html.form [
        Html.input [ prop.type'.checkbox ]
        Html.input [ prop.type' "password" ]
    ]

let staticHtml = React.functionComponent(fun () ->
    let html = Html.div [
        prop.style [ style.padding 20 ]
        prop.children [
            Html.h1 "Html content"
            Html.br [ ]
        ]
    ]


    Html.pre [
        Html.text (ReactDOMServer.renderToString html)
    ])


let staticMarkup = React.functionComponent(fun () ->
    let html = Html.div [
        prop.style [ style.padding 20 ]
        prop.children [
            Html.h1 "Html content"
            Html.br [ ]
        ]
    ]


    Html.pre [
        Html.text (ReactDOMServer.renderToStaticMarkup html)
    ])

let multipleStateVariables = React.functionComponent(fun () ->
    let (count, setCount) = React.useState(0)
    let (textColor, setTextColor) = React.useState(color.red)

    Html.div [
        Html.h1 [
            prop.style [ style.color textColor ]
            prop.text count
        ]

        Html.button [
            prop.text "Increment"
            prop.onClick (fun _ -> setCount(count + 1))
        ]

        Html.button [
            prop.text "Red"
            prop.onClick (fun _ -> setTextColor(color.red))
        ]

        Html.button [
            prop.text "Blue"
            prop.onClick (fun _ -> setTextColor(color.blue))
        ]
    ])

let asyncEffect = React.functionComponent(fun () ->
    let (isLoading, setLoading) = React.useState(false)
    let (content, setContent) = React.useState("")

    let loadData() = async {
        setLoading true
        do! Async.Sleep 1500
        setLoading false
        setContent "Content"
    }

    React.useEffect(loadData >> Async.StartImmediate, [||])

    Html.div [
        if isLoading
        then Html.h1 "Loading"
        else Html.h1 content
    ])

let asyncEffectOnce = React.functionComponent(fun () ->
    let (isLoading, setLoading) = React.useState(false)
    let (content, setContent) = React.useState("")

    let loadData() = async {
        setLoading true
        do! Async.Sleep 1500
        setLoading false
        setContent "Content"
    }

    React.useEffectOnce(loadData >> Async.StartImmediate)

    Html.div [
        if isLoading
        then Html.h1 "Loading"
        else Html.h1 content
    ])

[<Emit("setTimeout($0, $1)")>]
let setTimeout (f: unit -> unit) (timeout: int) : int = jsNative

[<Emit("clearTimeout($0)")>]
let clearTimeout (id: int) : unit = jsNative

let timer = React.functionComponent(fun () ->
    let (paused, setPaused) = React.useState(false)
    let (value, setValue) = React.useState(0)

    let subscribeToTimer() =
        // start the ticking
        let subscriptionId = setTimeout (fun _ -> if not paused then setValue (value + 1)) 1000
        // return IDisposable with cleanup code
        { new IDisposable with member this.Dispose() = clearTimeout(subscriptionId) }

    React.useEffect(subscribeToTimer)

    Html.div [
        Html.h1 value

        Html.button [
            prop.className [
                Bulma.Button
                Bulma.IsLarge
                if paused then Bulma.IsPrimary else Bulma.IsWarning
            ]

            prop.onClick (fun _ -> setPaused(not paused))
            prop.text (if paused then "Resume" else "Pause")
        ]
    ])

let rnd = System.Random()

let effectfulUserId = React.functionComponent(fun () ->
    let (isLoading, setLoading) = React.useState(false)
    let (content, setContent) = React.useState("")
    let (userId, setUserId) = React.useState(0)
    let (textColor, setTextColor) = React.useState(color.red)

    let loadData() = async {
        setLoading true
        do! Async.Sleep 1500
        setLoading false
        setContent (sprintf "User %d" userId)
    }

    React.useEffect(loadData >> Async.StartImmediate, [| box userId |])

    Html.div [
        Html.h1 [
            prop.style [ style.color textColor ]
            prop.text (if isLoading then "Loading" else content)
        ]

        Html.button [
            prop.text "Red"
            prop.onClick (fun _ -> setTextColor(color.red))
        ]

        Html.button [
            prop.text "Blue"
            prop.onClick (fun _ -> setTextColor(color.blue))
        ]

        Html.button [
            prop.text "Update User ID"
            prop.onClick (fun _ -> setUserId(rnd.Next(1, 100)))
        ]
    ])

let effectfulTabCounter = React.functionComponent(fun () ->
    let (count, setCount) = React.useState(0)

    // execute this effect on every render cycle
    React.useEffect(fun () -> Browser.Dom.document.title <- sprintf "Count = %d" count)

    Html.div [
        Html.h1 count
        Html.button [
            prop.text "Increment"
            prop.onClick (fun _ -> setCount(count + 1))
        ]
    ])

let counterApp state dispatch =
    Html.div [
        prop.id "main"
        prop.style [ style.padding 20 ]
        prop.children [

            Html.button [
                prop.style [ style.marginRight 5 ]
                prop.onClick (fun _ -> dispatch Increment)
                prop.text "Increment"
            ]

            Html.button [
                prop.style [ style.marginLeft 5 ]
                prop.onClick (fun _ -> dispatch Decrement)
                prop.text "Decrement"
            ]

            Html.h1 "Count"
        ]
    ]

let keyWarnings state dispatch =
    Html.div [
        prop.id "id"
        prop.className ["class"]
        prop.children [
            Html.div "text"
            Html.div [
                prop.id "id"
                prop.children [
                    Html.text "text"
                    Html.div [
                        prop.id "id"
                        prop.className "class"
                        prop.children [
                            Html.text "text"
                            Html.div [
                                prop.id "id"
                                prop.className "class"
                                prop.children [
                                    Html.text "text"
                                    Html.div []
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let fragmentTests =
    Html.div [
        prop.children [
            React.fragment [
                Html.h1 "One"
                Html.h2 "Two"
            ]
        ]
    ]

let animationsOnHover' = React.functionComponent(fun (props: {| content: ReactElement |}) -> [
    let (hovered, setHovered) = React.useState(false)
    Html.div [
        prop.style [
            style.padding 10
            style.transitionDuration (TimeSpan.FromMilliseconds 1000.0)
            style.transitionProperty [
                transitionProperty.backgroundColor
                transitionProperty.color
            ]

            if hovered then
               style.backgroundColor.lightBlue
               style.color.black
            else
               style.backgroundColor.limeGreen
               style.color.white
        ]
        prop.onMouseEnter (fun _ -> setHovered(true))
        prop.onMouseLeave (fun _ -> setHovered(false))
        prop.children [ props.content ]
    ]
])

let animationsOnHover content = animationsOnHover' {| content = React.fragment content |}
let animationSample =
    Html.div [
        animationsOnHover [ Html.span "Hover me!" ]
        animationsOnHover [ Html.p "So smooth" ]
    ]

[<ReactComponent>]
let SimpleDateInput() = 
    let (selectedDate, updateDate) = React.useState(DateTime.Now)
    Html.input [
        prop.type'.date
        prop.value selectedDate
        prop.onChange (fun newValue -> updateDate newValue)
    ]

[<ReactComponent>]
let SimpleDateAndTimeInput() = 
    let (selectedDate, updateDate) = React.useState(DateTime.Now)
    Html.input [
        prop.type'.dateTimeLocal
        prop.value(selectedDate, includeTime=true)
        prop.onChange  (fun newValue -> updateDate newValue)
    ]

let dateInputsExample = React.functionComponent(fun () -> 
    let (date, setDate) = React.useState<DateTime option>(None)
    let (dateAndTime, toggleDateAndTime) = React.useState(false)

    let formattedDate = 
        match date with 
        | None -> "No date selected yet"
        | Some date -> "Input: " + date.ToString "yyyy-MM-dd hh:mm"

    Html.div [

        Html.h3 formattedDate

        Html.input [
            prop.value(date, includeTime=dateAndTime)
            if dateAndTime then prop.type'.dateTimeLocal else prop.type'.date 
            prop.onChange (fun newValue -> setDate(Some newValue))
        ]

        Html.button [
            prop.text "Reset selected date"
            prop.disabled date.IsNone
            prop.onClick (fun _ -> setDate(None))
        ]

        Html.button [
            prop.text "Toggle date and time"
            prop.disabled date.IsNone
            prop.onClick (fun _ -> toggleDateAndTime(not dateAndTime))
        ]
    ])

let portalsPopup = React.functionComponent(fun () ->
    Html.div [
        prop.style [
            style.position.absolute
            style.top 10
            style.right 10
            style.padding 10
            style.backgroundColor.lightGreen
        ]
        prop.children [
            Html.p [
                prop.text "Portals can be used to escape the parent component."
            ]
        ]
    ]
)

let portalsPortal' = React.functionComponent(fun (props: {| content: ReactElement |}) ->
    let root = Browser.Dom.document.getElementById("root")
    ReactDOM.createPortal(props.content, root)
)

let portalsPortal content = portalsPortal' {| content = content |}

let portalsContainer = React.functionComponent(fun () ->
    Html.div [
        prop.style [
            style.padding 10
            style.overflow.hidden
        ]
        prop.children [ portalsPopup() |> portalsPortal ]
    ]
)

let portalsSample = portalsContainer()

module ReactComponents =
    type Greeting = { Name: string option }
    let greeting = React.functionComponent(fun (props: Greeting) ->
        Html.div [
            Html.span "Hello, "
            Html.span (Option.defaultValue "World" props.Name)
        ])

    let simple = Html.div [
        prop.className "content"
        prop.children [
            greeting { Name = Some "John" }
            greeting { Name = None }
        ]
    ]


    let counter = React.functionComponent(fun () ->
        let (count, setCount) = React.useState(0)
        Html.div [
            Html.h1 count
            Html.button [
                prop.text "Increment"
                prop.onClick (fun _ -> setCount(count + 1))
            ]
        ])

let View() = Html.h1 "My view"

#if FABLE_COMPILER_3 || FABLE_COMPILER_4
[<ReactComponent>]
let CounterExternal() =
    let (count, setCount) = React.useState 0

    Html.div [
        Html.h1 count
        Html.button [
            prop.text "Increment"
            prop.onClick (fun _ -> setCount(count + 1))
        ]
    ]
#endif

let counter = React.functionComponent(fun () ->
    let (count, setCount) = React.useState 0

    let modifyDocumentTitle() =
        Browser.Dom.window.document.title <- string count

    React.useEffect(modifyDocumentTitle)

    Html.div [
        Html.h1 count
        Html.button [
            prop.text "Increment"
            prop.onClick (fun _ -> setCount(count + 1))
        ]
    ])

[<Emit("setInterval($0, $1)")>]
let setInterval (f: unit -> unit) (n: int) : int = jsNative

[<Emit("clearInterval($0)")>]
let clearInterval (n: int) : unit = jsNative

let ticker = React.functionComponent("Ticker", fun (input: {| start: int |}) ->
    let (tick, setTick) = React.useState input.start

    let tickerEffect() : IDisposable =
        let interval = setInterval (fun () -> setTick(tick + 1)) 1000
        React.createDisposable(fun () -> clearInterval(interval))

    React.useEffect(tickerEffect, [| box input.start |])

    Html.div [
        Html.h1 tick
    ])

let hooksAreAwesome =
    React.fragment [
        counter()
        Html.hr [ ]
        ticker {| start = 0 |}
        Html.hr [ ]
        ticker {| start = 10 |}
    ]

module ElmishCounter =
    type State = { Count : int }
    type Msg = Increment | Decrement

    let initialState = { Count = 0 }

    let update (state: State) = function
        | Increment -> { state with Count = state.Count + 1 }
        | Decrement -> { state with Count = state.Count - 1 }

    let app = React.functionComponent("Counter", fun () ->
        let (state, dispatch) = React.useReducer(update, initialState)
        Html.div [
            Html.button [
                prop.onClick (fun _ -> dispatch Increment)
                prop.text "Increment"
            ]

            Html.button [
                prop.onClick (fun _ -> dispatch Decrement)
                prop.text "Decrement"
            ]

            Html.h1 state.Count
        ])

let focusInputExample = React.functionComponent(fun () ->
    let inputRef = React.useInputRef()
    let focusTextInput() = inputRef.current |> Option.iter (fun el -> el.focus())

    Html.div [
        Html.input [
            prop.style [ style.caretColor.red ]
            prop.ref inputRef
            prop.type'.text
        ]

        Html.button [
            prop.onClick (fun _ -> focusTextInput())
            prop.text "Focus Input"
        ]
    ])

let forwardRefChild = React.forwardRef(fun ((), ref) ->
    Html.input [
        prop.type'.text
        prop.ref ref
    ])

let forwardRefParent = React.functionComponent(fun () ->
    let inputRef = React.useInputRef()

    Html.div [
        forwardRefChild((), inputRef)
        Html.button [
            prop.text "Focus Input"
            prop.onClick <| fun ev ->
                inputRef.current
                |> Option.iter (fun elem -> elem.focus())
        ]
    ])

let forwardRefImperativeChild = React.forwardRef(fun ((), ref) ->
    let divText,setDivText = React.useState ""
    let inputRef = React.useInputRef()

    React.useImperativeHandle(ref, fun () ->
        inputRef.current
        |> Option.map(fun innerRef ->
            {| focus = fun () -> setDivText innerRef.className |})
    )

    Html.div [
        Html.input [
            prop.className "Howdy!"
            prop.type'.text
            prop.ref inputRef
        ]
        Html.div [
            prop.text divText
        ]
    ])

let forwardRefImperativeParent = React.functionComponent(fun () ->
    let ref = React.useRef<{| focus: unit -> unit |} option>(None)

    Html.div [
        forwardRefImperativeChild((), ref)
        Html.button [
            prop.text "Focus Input"
            prop.onClick <| fun ev ->
                ref.current
                |> Option.iter (fun elem -> elem.focus())
        ]
    ])

type StrictModeWarning () =
    inherit Fable.React.Component<obj,obj>()

    // The unsafe call.
    override _.componentWillMount() = ()

    override _.render () =
        Html.div [
            prop.text "I cause a warning!"
        ]

let strictModeExample = React.functionComponent(fun () ->
    Html.div [
        prop.style [
            style.display.inheritFromParent
        ]
        prop.children [
            React.strictMode [
                Fable.React.ReactBindings.React.createElement(
                    JsInterop.jsConstructor<StrictModeWarning>,
                    null,
                    []
                )
            ]
        ]
    ])

let myNonCodeSplitComponent = React.functionComponent(fun () ->
    Html.div [
        prop.text "I was loaded synchronously!"
    ])

let centeredSpinner =
    Html.div [
        prop.style [
            style.textAlign.center
            style.marginLeft length.auto
            style.marginRight length.auto
            style.marginTop 50
        ]
        prop.children [
            Html.li [
                prop.className [
                    FA.Fa
                    FA.FaRefresh
                    FA.FaSpin
                    FA.Fa3X
                ]
            ]
        ]
    ]

let asyncComponent : JS.Promise<unit -> ReactElement> = JsInterop.importDynamic "./CodeSplitting.fs"

let codeSplitting = React.functionComponent(fun () ->
    Html.div [
        prop.children [
            myNonCodeSplitComponent()
            React.suspense([
                Html.div [
                    React.lazy'(asyncComponent,())
                ]
            ], centeredSpinner)
        ]
    ])

let codeSplittingDelayed = React.functionComponent(fun () ->
    Html.div [
        prop.children [
            myNonCodeSplitComponent()
            React.suspense([
                Html.div [
                    React.lazy'((fun () ->
                        promise {
                            do! Promise.sleep 2000
                            return! asyncComponent
                        }
                    ),())
                ]
            ], centeredSpinner)
        ]
    ])

let rng = System.Random()

let sortNumbers () =
    Array.init 3000000 (fun _ -> rng.NextDouble() * 1000000.)
    |> Array.sort
    |> Array.sum
    |> int
    |> fun res ->
        JS.console.log(res)
        res

let useStateNormal = React.forwardRef(fun ((), ref) ->
    let count,setCount = React.useState (sortNumbers())

    let setCount = React.useCallback(setCount, [||])

    ref.current <- Some setCount

    Html.div [
        prop.classes [ Bulma.Box ]
        prop.children [
            Html.div [
                prop.text (sprintf "Normal Count: %i" count)
            ]
        ]
    ])

let useStateLazy = React.forwardRef(fun ((), ref) ->
    let count,setCount = React.useState (fun () -> sortNumbers())

    let setCount = React.useCallback(setCount, [||])

    ref.current <- Some setCount


    Html.div [
        prop.classes [ Bulma.Box ]
        prop.children [
            Html.div [
                prop.text (sprintf "Lazy Count: %i" count)
            ]
        ]
    ])

let useStateNormalVsLazy = React.functionComponent(fun () ->
    let rerenderNormal : IRefValue<(int -> unit) option> = React.useRef None
    let rerenderLazy : IRefValue<(int -> unit) option> = React.useRef None

    Html.div [
        prop.className Bulma.Control
        prop.style [
            style.paddingLeft (length.em 8)
            style.paddingBottom (length.em 1)
        ]
        prop.children [
            Html.div [
                prop.style [ style.maxWidth (length.em 15); style.paddingBottom (length.em 2) ]
                prop.children [
                    Html.div [
                        prop.style [
                            style.textAlign.center
                            style.marginLeft length.auto
                            style.marginRight length.auto
                            style.marginTop 50
                            style.paddingBottom (length.em 2)
                        ]
                        prop.children [ FPSStats.render() ]
                    ]
                    useStateNormal((), rerenderNormal)
                    useStateLazy((), rerenderLazy)
                ]
            ]

            Html.button [
                prop.classes [ Bulma.Button; Bulma.HasBackgroundPrimary; Bulma.HasTextWhite ]
                prop.onClick <| fun _ -> rerenderNormal.current |> Option.iter (fun f -> f(rng.Next(0,10)))
                prop.text "Rerender normal"
            ]
            Html.button [
                prop.classes [ Bulma.Button; Bulma.HasBackgroundPrimary; Bulma.HasTextWhite ]
                prop.onClick <| fun _ -> rerenderLazy.current |> Option.iter (fun f -> f(rng.Next(0,10)))
                prop.text "Rerender lazy"
            ]
        ]
    ])

module TokenCancellation =
    let useToken = React.functionComponent(fun (input: {| failedCallback: unit -> unit |}) ->
        let token = React.useCancellationToken()

        React.useEffect(fun () ->
            async {
                do! Async.Sleep 4000
                input.failedCallback()
            }
            |> fun a -> Async.StartImmediate(a,token.current)
        )

        Html.none)

    let result = React.functionComponent(fun (input: {| text: string |}) -> Html.div input.text)

    let render = React.functionComponent(fun () ->
        let renderChild,setRenderChild = React.useState true
        let resultText,setResultText = React.useState "Pending..."

        let setFailed = React.useCallbackRef <| fun () -> setResultText "You didn't cancel me in time!"

        Html.div [
            if renderChild then
                useToken {| failedCallback = setFailed |}
            result {| text = resultText |}
            Html.button [
                prop.classes [ Bulma.Button; Bulma.HasBackgroundPrimary; Bulma.HasTextWhite ]
                prop.disabled <| (resultText = "Disposed")
                prop.onClick <| fun _ ->
                    async {
                        setResultText "Disposed"
                        setRenderChild false
                    }
                    |> Async.StartImmediate
                prop.text "Dispose"
            ]
            Html.button [
                prop.classes [ Bulma.Button; Bulma.HasBackgroundPrimary; Bulma.HasTextWhite ]
                prop.disabled <| (renderChild && resultText = "Pending...")
                prop.onClick <| fun _ ->
                    async {
                        setResultText "Pending..."
                        setRenderChild true
                    }
                    |> Async.StartImmediate
                prop.text "Reset"
            ]
        ])
